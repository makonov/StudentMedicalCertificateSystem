using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAll();
        Task<Student> GetByIdAsync(int id);
        Task<Student> GetDefaultByFullName(string lastName, string  firstName, string patronymic);
        Task<List<Student>> GetAllByFullName(string lastName, string firstName, string patronymic);
        Task<List<Student>> GetAllByLastAndFirstNames(string lastName, string firstName);
        Task<List<Student>> GetAllByLastNameAndPatronymic(string lastName, string patronymic);
        Task<List<Student>> GetAllByLastName(string lastName);
        Task<List<Student>> GetAllByFirstNameAndPatronymic(string firstName, string patronymic);
        Task<List<Student>> GetAllByPatronymic(string patronymic);
        Task<List<Student>> GetAllByFirstName(string firstName);
        bool Add(Student student);
        bool Update(Student student);
        bool Delete(Student student);
        bool Save();
    }
}
