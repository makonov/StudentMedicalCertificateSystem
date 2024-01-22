using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IMedicalCertificateRepository
    {
        Task<List<MedicalCertificate>> GetAll();
        Task<List<MedicalCertificate>> GetAllSortedAndIncludedAsync();
        Task<MedicalCertificate> GetByIdAsync(int id);
        Task<MedicalCertificate> GetIncludedByIdAsync(int id);
        Task<MedicalCertificate> GetIncludedStudentByIdAsync(int id);
        Task<List<MedicalCertificate>> GetSortedAndIncludedFromList(List<MedicalCertificate> list);
        Task<List<MedicalCertificate>> GetAllByStudentId(int studentId);
        Task<List<MedicalCertificate>> GetAllByFullName(string lastName, string firstName, string patronymic);
        Task<List<MedicalCertificate>> GetAllByLastAndFirstNames(string lastName, string firstName);
        Task<List<MedicalCertificate>> GetAllByLastName(string lastName);
        Task<List<MedicalCertificate>> GetAllByTimePeriod(DateTime startOfPeriod, DateTime endOfPeriod);
        bool Add(MedicalCertificate certificate);
        bool Update(MedicalCertificate certificate);
        Task UpdateByAnotherCertificateValues(MedicalCertificate certificateToUpdate, MedicalCertificate updatedCertificate);
        bool Delete(MedicalCertificate certificate);
        bool Save();
    }
}
