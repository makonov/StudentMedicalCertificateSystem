using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IClinicRepository
    {
        Task<List<Clinic>> GetAll();
        Task<List<SelectListItem>> GetClinicsAsSelectList();
        Task<Clinic> GetByIdAsync(int id);
        bool Add(Clinic clinic);
        bool Update(Clinic clinic);
        bool Delete(Clinic clinic);
        bool Save();
    }
}
