using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IDiagnosisRepository
    {
        Task<List<Diagnosis>> GetAllSortedAsync();
        Task<List<SelectListItem>> GetDiagnosesAsSelectListAsync();
        Task<Diagnosis> GetByIdAsync(int id);
        bool DiagnosisExistsByName(string name);
        bool DiagnosisExistsByCode(string code);
        bool Add(Diagnosis diagnosis);
        bool Update(Diagnosis diagnosis);
        bool Delete(Diagnosis diagnosis);
        bool Save();
    }
}
