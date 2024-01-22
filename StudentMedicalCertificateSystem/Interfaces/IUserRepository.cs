using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();
        Task<List<SelectListItem>> GetUsersAsSelectList();
        Task<User> GetByIdAsync(string id);
        bool Add(User user);
        bool Update(User user);
        bool Delete(User user);
        bool Save();
    }
}
