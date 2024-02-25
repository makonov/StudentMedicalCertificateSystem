using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IEducationalProgramRepository
    {
        Task<List<EducationalProgram>> GetAllSorted();
        Task<List<SelectListItem>> GetProgramsAsSelectList();
        Task<EducationalProgram> GetByIdAsync(int id);
        bool Add(EducationalProgram program);
        bool Update(EducationalProgram program);
        bool Delete(EducationalProgram program);
        bool Save();
    }
}
