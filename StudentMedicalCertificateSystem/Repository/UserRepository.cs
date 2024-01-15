using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using static System.Net.Mime.MediaTypeNames;

namespace StudentMedicalCertificateSystem.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public bool Add(Users user)
        {
            _context.Add(user);
            return Save();
        }

        public bool Delete(Users user)
        {
            _context.Remove(user);
            return Save();
        }

        public async Task<List<Users>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<Users> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task<List<SelectListItem>> GetUsersAsSelectList()
        {
            return await _context.Users.Select(u => new SelectListItem { Value = u.UserID.ToString(), Text = u.LastName + " " + u.FirstName + " " + u.Patronymic }).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Users user)
        {
            _context.Update(user);
            return Save();
        }
    }
}
