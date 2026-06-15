using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp.PixelFormats;

namespace HRemployee.Helper
{
    public class CheckInput
    {
        public static string IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "Tên đăng nhập không được để trống.";
            if (username.Length < 3 || username.Length > 50)
                return "Tên đăng nhập phải có từ 3 đến 50 ký tự.";
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c))
                    return "Tên đăng nhập chỉ được chứa các ký tự chữ và số.";
            }
            return username;
        }

        public static string IsPassWord(string password)
        {
            Regex regex = new Regex("[!@#$%^&*()_+{}\\[\\]:;<>,.?/~`]");
            if (password.Length < 8 || password.Length > 20 || !regex.IsMatch(password))
                return "Password phải từ 8-20 ký tự và chứa ít nhất 1 ký tự đặc biệt!";
            return password;
        }

        public static bool IsValidEmail(string email)
        {
            var check = new EmailAddressAttribute();
            return check.IsValid(email);
        }

        public static string IsValidPhoneNumber(string phoneNumber)
        {
            if (phoneNumber == null) return "Số điện thoại không được để trống!"; if (!Regex.IsMatch(phoneNumber, @"^\+?[0-9]{10,15}$"))
                return "Số điện thoại không hợp lệ. Phải từ 10-15 chữ số.";
            return phoneNumber;
        }

        public static bool IsImage(IFormFile imageFile)
        {
            int maxSizeInBytes = (2 * 1024 * 768);
            try
            {
                using (var image = SixLabors.ImageSharp.Image.Load<Rg32>(imageFile.OpenReadStream()))
                {
                    if (image.Width > 0 && image.Height > 0)
                    {
                        if (imageFile.Length <= maxSizeInBytes) return true;
                        throw new NotImplementedException("Kích thước file quá lớn");
                    }
                }
            }
            catch
            {
                if (imageFile.Length > maxSizeInBytes)
                    throw new NotImplementedException("Kích thước file quá lớn");
                else
                    throw new NotImplementedException("File này không phải file ảnh hợp lệ");
            }
            return false;
        }

        public static bool IsExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;
            var allowedExtensions = new[] { ".xls", ".xlsx" };
            return allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
        }
    }
}