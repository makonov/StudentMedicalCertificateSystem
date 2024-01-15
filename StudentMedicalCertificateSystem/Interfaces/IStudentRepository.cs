using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Students>> GetAll();
        Task<Students> GetByIdAsync(int id);
        Task<Students> GetDefaultByFullName(string lastName, string  firstName, string patronymic);
        Task<List<Students>> GetAllByFullName(string lastName, string firstName, string patronymic);
        Task<List<Students>> GetAllByLastAndFirstNames(string lastName, string firstName);
        Task<List<Students>> GetAllByLastNameAndPatronymic(string lastName, string patronymic);
        Task<List<Students>> GetAllByLastName(string lastName);
        Task<List<Students>> GetAllByFirstNameAndPatronymic(string firstName, string patronymic);
        Task<List<Students>> GetAllByPatronymic(string patronymic);
        Task<List<Students>> GetAllByFirstName(string firstName);
        bool Add(Students student);
        bool Update(Students student);
        bool Delete(Students student);
        bool Save();
    }
}
