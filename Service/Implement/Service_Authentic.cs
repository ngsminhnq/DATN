using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.Helper;
using HRemployee.PayLoad.Converter;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;
using HRemployee.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HRemployee.Service.Implement
{
    public class Service_Authentic : IService_Authentic
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly ResponseBase _responseBase;
        private readonly ResponseObject<DTO_Token> _responseToken;
        private readonly ResponseObject<object> _responseObj;
        private readonly Converter_Login _converter;

        public Service_Authentic(
            AppDbContext db,
            IConfiguration config,
            ResponseBase responseBase,
            ResponseObject<DTO_Token> responseToken,
            ResponseObject<object> responseObj,
            Converter_Login converter)
        {
            _db = db;
            _config = config;
            _responseBase = responseBase;
            _responseToken = responseToken;
            _responseObj = responseObj;
            _converter = converter;
        }

        public async Task<ResponseObject<DTO_Token>> Login(Request_Login request)
        {
            var user = await _db.Users.Include(u => u.Role).Include(u => u.Employee).FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null) return _responseToken.ResponseObjectError(404, "Tài khoản không tồn tại!", null);

            if (!user.IsActive)
                return _responseToken.ResponseObjectError(400, "Tài khoản đã bị khóa!", null);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return _responseToken.ResponseObjectError(400, "Sai mật khẩu!", null);

            if (user.EmployeeId.HasValue)
            {
                var hasActiveContract = await _db.Contracts.AnyAsync(c => c.EmployeeId == user.EmployeeId && c.Status == Enums.ContractStatusEnum.Active);

                if (!hasActiveContract)
                    return _responseToken.ResponseObjectError(400, "Hợp đồng lao động đã hết hạn! Vui lòng liên hệ phòng nhân sự.", null);
            }

            return _responseToken.ResponseObjectSuccess("Đăng nhập thành công!", GenerateAccessToken(user));
        }

        public async Task<ResponseObject<DTO_Token>> RenewAccessToken(DTO_Token request)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenValidation = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, ValidateAudience = false, ValidateIssuer = false, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:SecretKey").Value!)), ValidateLifetime = false };

            try
            {
                var principal = jwtHandler.ValidateToken(request.AccessToken, tokenValidation, out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwt || jwt.Header.Alg != SecurityAlgorithms.HmacSha256) return _responseToken.ResponseObjectError(400, "Access token không hợp lệ!", null);

                var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
                if (expClaim != null && long.TryParse(expClaim, out long exp))
                {
                    if (DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime > DateTime.UtcNow)
                        return _responseToken.ResponseObjectError(400, "Access token chưa hết hạn!", null);
                }

                var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

                if (refreshToken == null) return _responseToken.ResponseObjectError(404, "Refresh token không tồn tại!", null);

                if (refreshToken.IsRevoked)
                    return _responseToken.ResponseObjectError(401, "Refresh token đã bị thu hồi!", null);

                if (refreshToken.ExpiryDate < DateTime.UtcNow)
                    return _responseToken.ResponseObjectError(401, "Refresh token đã hết hạn!", null);

                var user = await _db.Users.Include(u => u.Role).Include(u => u.Employee).FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);

                if (user == null) return _responseToken.ResponseObjectError(404, "Người dùng không tồn tại!", null);

                refreshToken.IsRevoked = true;
                _db.RefreshTokens.Update(refreshToken);

                var newToken = GenerateAccessToken(user);
                await _db.SaveChangesAsync();

                return _responseToken.ResponseObjectSuccess("Làm mới token thành công!", newToken);
            }
            catch (Exception ex)
            {
                return _responseToken.ResponseObjectError(500, ex.Message, null);
            }
        }

        public async Task<ResponseBase> ChangePassword(Request_ChangePassword request, int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return _responseBase.ResponseBaseError(404, "Người dùng không tồn tại!");

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
                return _responseBase.ResponseBaseError(400, "Mật khẩu cũ không chính xác!");

            if (!request.NewPassword.Equals(request.ConfirmPassword))
                return _responseBase.ResponseBaseError(400, "Mật khẩu mới và xác nhận không trùng nhau!");

            if (request.NewPassword.Equals(request.OldPassword))
                return _responseBase.ResponseBaseError(400, "Mật khẩu mới không được trùng mật khẩu cũ!");

            if (!CheckInput.IsPassWord(request.NewPassword).Equals(request.NewPassword))
                return _responseBase.ResponseBaseError(400, CheckInput.IsPassWord(request.NewPassword));

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return _responseBase.ResponseBaseSuccess("Đổi mật khẩu thành công!");
        }

        public async Task<ResponseBase> CreateAccount(Request_CreateUser request)
        {
            if (string.IsNullOrWhiteSpace(request.EmployeeCode))
                return _responseBase.ResponseBaseError(400, "Vui lòng nhập mã nhân viên!");

            if (string.IsNullOrWhiteSpace(request.Username))
                return _responseBase.ResponseBaseError(400, "Vui lòng nhập tên đăng nhập!");

            if (request.RoleId <= 0) return _responseBase.ResponseBaseError(400, "Vui lòng chọn vai trò hợp lệ (1=Director, 2=BlockManager, 3=CenterManager, 4=Employee)!");

            if (!await _db.Roles.AnyAsync(r => r.Id == request.RoleId)) return _responseBase.ResponseBaseError(404, $"Vai trò Id={request.RoleId} không tồn tại!");

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == request.EmployeeCode && !e.IsDeleted);

            if (employee == null) return _responseBase.ResponseBaseError(404, $"Không tìm thấy nhân viên với mã '{request.EmployeeCode}'!");

            if (await _db.Users.AnyAsync(u => u.EmployeeId == employee.Id)) return _responseBase.ResponseBaseError(400, $"Nhân viên '{employee.FullName}' ({request.EmployeeCode}) đã có tài khoản rồi!");

            if (await _db.Users.AnyAsync(u => u.Username == request.Username)) return _responseBase.ResponseBaseError(400, $"Tên đăng nhập '{request.Username}' đã tồn tại, hãy chọn tên khác!");

            if (await _db.Users.AnyAsync(u => u.Email == employee.Email)) return _responseBase.ResponseBaseError(400, $"Email '{employee.Email}' của nhân viên đã gắn với tài khoản khác! Hãy cập nhật email trong hồ sơ nhân viên trước.");

            string tempPassword = GenerateRandomPassword();

            var newUser = new User
            {
                Username = request.Username.Trim(), Email = employee.Email, Password = BCrypt.Net.BCrypt.HashPassword(tempPassword), IsActive = true, RoleId = request.RoleId, EmployeeId = employee.Id, CreatedAt = DateTime.UtcNow };
            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();

            var emailTo = new EmailTo
            {
                Mail = employee.Email, Subject = "TÀI KHOẢN HỆ THỐNG NHÂN SỰ", Content = $@" <h3>Xin chào {employee.FullName},</h3> <p>Tài khoản hệ thống của bạn đã được tạo.</p> <p><strong>Tên đăng nhập:</strong> {request.Username}</p> <p><strong>Mật khẩu tạm thời:</strong> {tempPassword}</p> <p style='color:red'>Vui lòng đăng nhập và đổi mật khẩu ngay!</p>" };
            await emailTo.SendEmailAsync(emailTo);

            Console.WriteLine($"[DEBUG] CreateAccount -> NV: {employee.EmployeeCode} | Username: {request.Username} | TempPassword: {tempPassword}");

            return _responseBase.ResponseBaseSuccess($"Tạo tài khoản thành công cho '{employee.FullName}'! Mật khẩu tạm thời đã gửi tới {employee.Email}");
        }

        public async Task<ResponseBase> LockAccount(int userId, bool isLocked)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return _responseBase.ResponseBaseError(404, "Người dùng không tồn tại!");

            user.IsActive = !isLocked;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return _responseBase.ResponseBaseSuccess(isLocked ? "Đã khóa tài khoản thành công!" : "Đã mở khóa tài khoản thành công!");
        }

        public async Task<ResponseBase> ResetPassword(int userId)
        {
            var user = await _db.Users.Include(u => u.Employee).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return _responseBase.ResponseBaseError(404, "Người dùng không tồn tại!");

            string newPassword = GenerateRandomPassword();

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            var emailTo = new EmailTo
            {
                Mail = user.Employee?.Email ?? user.Email, Subject = "[HỆ THỐNG NHÂN SỰ] MẬT KHẨU ĐÃ ĐƯỢC THIẾT LẬP", Content = $@" <h3>Xin chào {user.Employee?.FullName ?? user.Username},</h3> <p>Mật khẩu tài khoản của bạn vừa được quản trị viên thiết lập.</p> <p><strong>Tên đăng nhập:</strong> {user.Username}</p> <p><strong>Mật khẩu mới:</strong> {newPassword}</p> <p style='color:red'>Vui lòng đăng nhập và đổi mật khẩu ngay!</p>" };
            await emailTo.SendEmailAsync(emailTo);

            return _responseBase.ResponseBaseSuccess($"Reset mật khẩu thành công! Mật khẩu mới đã gửi tới {user.Employee?.Email ?? user.Email}");
        }

        public async Task<ResponseObject<object>> DecodeJwtTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return await Task.FromResult(_responseObj.ResponseObjectError(400, "Token không được để trống!", null));

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                var claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value);

                var expClaim = claims.GetValueOrDefault("exp");
                if (!string.IsNullOrEmpty(expClaim))
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
                    if (expDate < DateTime.UtcNow)
                        return _responseObj.ResponseObjectError(401, "Token đã hết hạn!", null);
                }

                var data = new Dictionary<string, string>();
                if (claims.ContainsKey("Id")) data["Id"] = claims["Id"];
                if (claims.ContainsKey("username")) data["Username"] = claims["username"];
                if (claims.ContainsKey("Email")) data["Email"] = claims["Email"];
                if (claims.ContainsKey("RoleId")) data["RoleId"] = claims["RoleId"];
                if (claims.ContainsKey("EmployeeId")) data["EmployeeId"] = claims["EmployeeId"];
                if (claims.ContainsKey(ClaimTypes.Role)) data["RoleName"] = claims[ClaimTypes.Role];

                return _responseObj.ResponseObjectSuccess("Giải mã token thành công!", data);
            }
            catch
            {
                return _responseObj.ResponseObjectError(400, "Token không hợp lệ!", null);
            }
        }

        private DTO_Token GenerateAccessToken(User user)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:SecretKey").Value!);
            var role = _db.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("username", user.Username),
                    new Claim("Email", user.Email),
                    new Claim("RoleId", user.RoleId.ToString()),
                    new Claim("EmployeeId", user.EmployeeId?.ToString() ?? ""),
                    new Claim(ClaimTypes.Role, role?.Name ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(12), SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature) };

            var accessToken = jwtHandler.WriteToken(jwtHandler.CreateToken(tokenDescriptor));
            var newRefreshToken = GenerateRefreshToken();

            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken, ExpiryDate = DateTime.UtcNow.AddDays(7), IsRevoked = false, UserId = user.Id });
            _db.SaveChanges();

            return new DTO_Token
            {
                AccessToken = accessToken, RefreshToken = newRefreshToken };
        }

        private static string GenerateRefreshToken()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }

        private static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            const string special = "@#$!";
            var rnd = new Random();
            var base_ = new string(Enumerable.Repeat(chars, 8).Select(s => s[rnd.Next(s.Length)]).ToArray());
            return "Hr@" + base_;
        }

        public async Task<ResponseBase> LockAccountByEmployeeCode(string employeeCode, bool isLocked)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseBase.ResponseBaseError(404, $"Không tìm thấy nhân viên '{employeeCode}'");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.EmployeeId == employee.Id);
            if (user == null) return _responseBase.ResponseBaseError(404, $"Nhân viên '{employeeCode}' chưa có tài khoản");

            return await LockAccount(user.Id, isLocked);
        }

        public async Task<ResponseBase> ResetPasswordByEmployeeCode(string employeeCode)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseBase.ResponseBaseError(404, $"Không tìm thấy nhân viên '{employeeCode}'");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.EmployeeId == employee.Id);
            if (user == null) return _responseBase.ResponseBaseError(404, $"Nhân viên '{employeeCode}' chưa có tài khoản");

            return await ResetPassword(user.Id);
        }
    }
}