namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IPhotoService
    {
        Task<(bool IsExtensionValid, string FileName)> UploadPhotoAsync(IFormFile file, string targetFolder);
        Task<(bool IsReplacementSuccess, string NewFileName)> ReplacePhotoAsync(IFormFile file, string targetFolder, string existingFilePath);
        Task<bool> DeletePhotoAsync(string filePath);
        bool IsFileExtensionAllowed(IFormFile file, string[] allowedExtensions);
    }
}
