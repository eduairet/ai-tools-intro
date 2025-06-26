namespace EventManagementApi.Shared.Helpers;

public static partial class Helpers
{
    public static class File
    {
        public static bool IsValidImage(IFormFile file)
        {
            if (file.Length == 0) return false;

            var allowedExtensions = Constants.Constants.Api.FileUpload
                .AllowedExtensions.Split(',');
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return file.Length <= Constants.Constants.Api.FileUpload.MaxFileSizeMb * 1024 * 1024 &&
                   allowedExtensions.Contains(fileExtension);
        }

        public static async Task<string> SaveAsync(IFormFile file, string uploadPath)
        {
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        public static string GetTempUploadPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                Constants.Constants.Api.FileUpload.TempFolder);
        }
    }
}