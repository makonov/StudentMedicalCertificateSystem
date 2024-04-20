using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IClinicRepository
    {
        Task<List<Clinic>> GetAllSortedAsync();
        Task<List<SelectListItem>> GetClinicsAsSelectListAsync();
        Task<Clinic> GetByIdAsync(int id);
        bool ClinicExists(string name);
        bool Add(Clinic clinic);
        bool Update(Clinic clinic);
        bool Delete(Clinic clinic);
        bool Save();
    }
}
