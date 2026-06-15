using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Text.RegularExpressions;

namespace HRemployee.Helper
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var account = new Account("dkh1dujcl", "327523652721919", "Ck64akZQteJXIMu5m3bLNLpzW_E");
            _cloudinary = new Cloudinary(account);
        }

        public async Task<bool> DeleteImage(string publicId)
        {
            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image };
            var result = await _cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok";
        }

        public async Task<string> UploadImage(IFormFile newImage)
        {
            using (var stream = newImage.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(newImage.FileName, stream) };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<string> ReplaceImage(string url, IFormFile newImage)
        {
            var parts = url.Split(new[] { "image/upload/" }, StringSplitOptions.None);
            if (parts.Length < 2) return null;
            var publicIdWithExtension = parts[1].Substring(parts[1].IndexOf('/') + 1);

            var oldPublicId = publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));
            bool isDeleted = await DeleteImage(oldPublicId);

            if (!isDeleted)
            {
                throw new Exception("Failed to delete the old image from Cloudinary.");
            }

            string newImageUrl = await UploadImage(newImage);

            return newImageUrl;
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("Tệp không hợp lệ.");

            using (var stream = file.OpenReadStream())
            {
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName).Trim();

                var safeFileName = System.Text.RegularExpressions.Regex.Replace(originalFileName, @"[^a-zA-Z0-9_\-]", "_");

                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(file.FileName, stream), PublicId = safeFileName };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                {
                    throw new Exception($"Upload lỗi: {result.Error.Message}");
                }

                if (result.SecureUrl == null)
                {
                    throw new Exception("Tải lên thất bại: không có SecureUrl được trả về.");
                }

                return result.SecureUrl.ToString();
            }
        }

        public async Task<string> ReplaceFile(string url, IFormFile newFile)
        {
            var oldPublicId = ExtractPublicIdFromUrl(url);
            if (string.IsNullOrEmpty(oldPublicId)) return null;

            bool deleted = await DeleteFile(oldPublicId);
            if (!deleted) throw new Exception("Failed to delete old file.");

            return await UploadFile(newFile);
        }

        public string ExtractPublicIdFromUrl(string url)
        {
            try
            {
                var parts = url.Split(new[] { "raw/upload/" }, StringSplitOptions.None);
                if (parts.Length < 2) return null;
                var publicIdWithExtension = parts[1].Substring(parts[1].IndexOf('/') + 1);

                var oldPublicId = publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));

                return oldPublicId;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteFile(string publicUrl)
        {
            var publicId = ExtractPublicIdFromUrl(publicUrl);
            if (string.IsNullOrEmpty(publicId))
            {
                Console.WriteLine("PublicId không hợp lệ.");
                return false;
            }

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Raw };

            var result = await _cloudinary.DestroyAsync(deletionParams);
            return result.Result == "ok";
        }

        public string GetEmbeddedUrl(string urlFile)
        {
            if (string.IsNullOrWhiteSpace(urlFile))
                return null;

            var urlWithNoSpaces = urlFile.Replace(" ", "%20");

            if (urlWithNoSpaces.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return urlWithNoSpaces;
            }
            else
            {
                return $"https://view.officeapps.live.com/op/embed.aspx?src={Uri.EscapeDataString(urlWithNoSpaces)}";
            }
        }
    }
}