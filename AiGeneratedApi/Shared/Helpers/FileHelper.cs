using Microsoft.AspNetCore.Http;
using EventManagementApi.Shared.Constants;

namespace EventManagementApi.Shared.Helpers
{
    public static partial class Helpers
    {
        public static class File
        {
            public static bool IsValidImage(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    return false;
                var allowedExtensions = EventManagementApi.Shared.Constants.Constants.ApiConstants.FileUpload.AllowedExtensions.Split(',');
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return allowedExtensions.Contains(fileExtension);
            }
            public static async Task<string> SaveAsync(IFormFile file, string uploadPath)
            {
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return fileName;
            }
            public static string GetTempUploadPath()
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", EventManagementApi.Shared.Constants.Constants.ApiConstants.FileUpload.TempFolder);
            }
        }
    }
} 