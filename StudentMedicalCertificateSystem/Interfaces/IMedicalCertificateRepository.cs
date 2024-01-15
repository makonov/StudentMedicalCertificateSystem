using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IMedicalCertificateRepository
    {
        Task<List<MedicalCertificates>> GetAll();
        Task<List<MedicalCertificates>> GetAllSortedAndIncludedAsync();
        Task<MedicalCertificates> GetByIdAsync(int id);
        Task<MedicalCertificates> GetIncludedByIdAsync(int id);
        Task<MedicalCertificates> GetIncludedStudentByIdAsync(int id);
        Task<List<MedicalCertificates>> GetAllByStudentId(int studentId);
        Task<List<MedicalCertificates>> GetAllByFullName(string lastName, string firstName, string patronymic);
        Task<List<MedicalCertificates>> GetAllByLastAndFirstNames(string lastName, string firstName);
        Task<List<MedicalCertificates>> GetAllByLastName(string lastName);
        Task<List<MedicalCertificates>> GetAllByTimePeriod(DateTime startOfPeriod, DateTime endOfPeriod);
        bool Add(MedicalCertificates certificate);
        bool Update(MedicalCertificates certificate);
        Task UpdateByAnotherCertificateValues(MedicalCertificates certificateToUpdate, MedicalCertificates updatedCertificate);
        bool Delete(MedicalCertificates certificate);
        bool Save();
    }
}
