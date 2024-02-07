using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAll();
        Task<int> Count();
        Task<List<Student>> GetAllIncludedGroupAsync();
        Task<List<Student>> GetAllSortedAndIncludedAsync();
        Task<Student> GetIncludedByIdAsync(int id);
        Task<Student> GetByIdAsync(int id);
        Task<List<Student>> GetPagedStudents(int page, int pageSize);
        Task<List<SelectListItem>> GetStudentFullNamesWithGroupsAsSelectedList();
        Task<Student> GetDefaultByFullName(string lastName, string  firstName, string patronymic);
        Task<Student> GetByFullNameAndGroup(string lastName, string firstName, string patronymic, string group);
        bool Add(Student student);
        bool Update(Student student);
        bool Delete(Student student);
        bool Save();
    }
}
