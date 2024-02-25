using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IMedicalCertificateRepository
    {
        Task<List<MedicalCertificate>> GetAll();
        Task<int> Count();
        Task<List<MedicalCertificate>> GetAllSortedAndIncludedAsync();
        Task<List<MedicalCertificate>> GetAllIncludedAsync();
        Task<MedicalCertificate> GetByIdAsync(int id);
        Task<MedicalCertificate> GetIncludedByIdAsync(int id);
        Task<List<MedicalCertificate>> GetAllByProgramIdAsync(int id);
        Task<List<MedicalCertificate>> GetAllSortedAndIncludedByStudentIdAsync(int id);
        Task<List<MedicalCertificate>> GetSortedAndIncludedFromList(List<MedicalCertificate> list);
        Task<List<MedicalCertificate>> GetAllByStudentId(int studentId);
        Task<List<MedicalCertificate>> GetAllByTimePeriod(DateTime startOfPeriod, DateTime endOfPeriod);
        Task<List<MedicalCertificate>> GetPagedCertificates(int page, int pageSize);
        Task<List<MedicalCertificate>> GetPagedCertificatesFromList(List<MedicalCertificate> certificates, int page, int pageSize);
        bool Add(MedicalCertificate certificate);
        bool Update(MedicalCertificate certificate);
        bool UpdateByAnotherCertificateValues(MedicalCertificate certificateToUpdate, MedicalCertificate updatedCertificate);
        bool Delete(MedicalCertificate certificate);
        bool Save();
    }
}
