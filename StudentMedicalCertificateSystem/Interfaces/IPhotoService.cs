namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IPhotoService
    {
        Task<(bool IsUploadedAndExtensionValid, string FileName)> UploadPhotoAsync(IFormFile file, string targetFolder);
        Task<(bool IsReplacementSuccess, string NewFileName)> ReplacePhotoAsync(IFormFile file, string targetFolder, string existingFilePath);
        bool DeletePhoto(string filePath);
        bool IsFileUploadedAndExtensionAllowed(IFormFile file, string[] allowedExtensions);
    }
}
