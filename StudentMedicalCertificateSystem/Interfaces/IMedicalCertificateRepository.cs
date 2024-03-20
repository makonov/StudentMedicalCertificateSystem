using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IMedicalCertificateRepository
    {
        Task<int> CountAsync();
        Task<List<MedicalCertificate>> GetAllSortedAndIncludedAsync();
        Task<List<MedicalCertificate>> GetAllIncludedAsync();
        Task<MedicalCertificate> GetByIdAsync(int id);
        Task<MedicalCertificate> GetIncludedByIdAsync(int id);
        Task<List<MedicalCertificate>> GetAllByProgramIdAsync(int id);
        Task<List<MedicalCertificate>> GetAllSortedAndIncludedByStudentIdAsync(int id);
        Task<List<MedicalCertificate>> GetAllByTimePeriodAsync(DateTime startOfPeriod, DateTime endOfPeriod);
        Task<List<MedicalCertificate>> GetPagedCertificatesAsync(int page, int pageSize);
        bool Add(MedicalCertificate certificate);
        bool Update(MedicalCertificate certificate);
        bool Delete(MedicalCertificate certificate);
        bool Save();
    }
}
