using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IEducationalOfficeRepository
    {
        Task<List<EducationalOffice>> GetAll();
        Task<List<SelectListItem>> GetOfficesAsSelectList();
        Task<EducationalOffice> GetByIdAsync(int id);
        bool Add(EducationalOffice office);
        bool Update(EducationalOffice user);
        bool Delete(EducationalOffice user);
        bool Save();
    }
}
