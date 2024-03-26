using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IStudentGroupRepository
    {
        Task<List<StudentGroup>> GetAllSortedAsync();
        Task<List<SelectListItem>> GetGroupsAsSelectListAsync();
        Task<StudentGroup> GetByIdAsync(int id);
        bool Add(StudentGroup group);
        bool Update(StudentGroup group);
        bool Delete(StudentGroup group);
        bool Save();
    }
}
