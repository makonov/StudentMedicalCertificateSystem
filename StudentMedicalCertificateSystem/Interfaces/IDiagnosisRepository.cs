using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IDiagnosisRepository
    {
        Task<List<Diagnoses>> GetAll();
        Task<List<SelectListItem>> GetDiagnosesAsSelectList();
        Task<Diagnoses> GetByIdAsync(int id);
        bool Add(Diagnoses diagnosis);
        bool Update(Diagnoses diagnosis);
        bool Delete(Diagnoses diagnosis);
        bool Save();
    }
}
