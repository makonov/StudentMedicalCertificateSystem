using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IEducationalProgramRepository
    {
        Task<List<EducationalProgram>> GetAllSortedAsync();
        Task<List<SelectListItem>> GetProgramsAsSelectListAsync();
        Task<EducationalProgram> GetByIdAsync(int id);
        bool ProgramExists(string name);
        bool Add(EducationalProgram program);
        bool Update(EducationalProgram program);
        bool Delete(EducationalProgram program);
        bool Save();
    }
}
