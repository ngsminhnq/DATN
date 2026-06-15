using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.PayLoad.DTO;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_Login
    {
        private readonly AppDbContext _context;

        public Converter_Login(AppDbContext context)
        {
            _context = context;
        }

        public DTO_Login EntityToDTO(User user, string accessToken = "", string refreshToken = "")
        {
            var employee = user.Employee;
            return new DTO_Login
            {
                UserId = user.Id, Username = user.Username, Email = user.Email, RoleName = user.Role?.Name ?? "", EmployeeId = user.EmployeeId, FullName = employee?.FullName ?? "", AccessToken = accessToken, RefreshToken = refreshToken };
        }
    }
}