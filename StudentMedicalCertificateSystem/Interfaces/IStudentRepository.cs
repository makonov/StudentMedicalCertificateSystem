using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAll();
        Task<List<Student>> GetAllIncludedGroupAsync();
        Task<List<Student>> GetAllSortedAndIncludedAsync();
        Task<Student> GetByIdAsync(int id);
        Task<List<SelectListItem>> GetStudentFullNamesWithGroupsAsSelectedList();
        Task<Student> GetDefaultByFullName(string lastName, string  firstName, string patronymic);
        bool Add(Student student);
        bool Update(Student student);
        bool Delete(Student student);
        bool Save();
    }
}
