using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IUserRepository
    {
        Task<List<Users>> GetAll();
        Task<List<SelectListItem>> GetUsersAsSelectList();
        Task<Users> GetByIdAsync(int id);
        bool Add(Users user);
        bool Update(Users user);
        bool Delete(Users user);
        bool Save();
    }
}
