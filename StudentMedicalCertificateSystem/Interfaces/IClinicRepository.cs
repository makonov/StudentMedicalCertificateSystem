using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IClinicRepository
    {
        Task<List<Clinics>> GetAll();
        Task<List<SelectListItem>> GetClinicsAsSelectList();
        Task<Clinics> GetByIdAsync(int id);
        bool Add(Clinics clinic);
        bool Update(Clinics clinic);
        bool Delete(Clinics clinic);
        bool Save();
    }
}
