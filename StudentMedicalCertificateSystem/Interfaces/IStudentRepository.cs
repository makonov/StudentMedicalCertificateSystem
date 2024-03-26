using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IStudentRepository
    {
        Task<int> Count();
        Task<Student> GetIncludedByIdAsync(int id);
        Task<List<Student>> GetPagedStudentsAsync(int page, int pageSize);
        Task<List<SelectListItem>> GetFullNamesWithGroupsAsSelectedListAsync();
        Task<Student> GetDefaultByFullNameAsync(string lastName, string  firstName, string patronymic);
        Task<Student> GetByFullNameAndGroupAsync(string lastName, string firstName, string patronymic, string group);
        bool Add(Student student);
        bool Update(Student student);
        bool Delete(Student student);
        bool Save();
    }
}
