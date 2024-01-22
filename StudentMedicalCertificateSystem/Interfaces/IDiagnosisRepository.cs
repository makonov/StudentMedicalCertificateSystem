using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IDiagnosisRepository
    {
        Task<List<Diagnosis>> GetAll();
        Task<List<SelectListItem>> GetDiagnosesAsSelectList();
        Task<Diagnosis> GetByIdAsync(int id);
        bool Add(Diagnosis diagnosis);
        bool Update(Diagnosis diagnosis);
        bool Delete(Diagnosis diagnosis);
        bool Save();
    }
}
