using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Repository
{
    public class StudentGroupRepository : IStudentGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentGroupRepository(ApplicationDbContext context)
        { 
            _context = context;
        }

        public bool Add(StudentGroup group)
        {
            _context.Add(group);
            return Save();
        }

        public bool Delete(StudentGroup group)
        {
            _context.Remove(group);
            return Save();
        }

        public async Task<List<StudentGroup>> GetAllSortedAsync()
        {
            return await _context.StudentGroups
                .Include(g => g.Program)
                .OrderBy(g => g.GroupName).ToListAsync();
        }

        public async Task<StudentGroup> GetByIdAsync(int id)
        {
            return await _context.StudentGroups
                .Include(g => g.Program)
                .FirstOrDefaultAsync(g => g.GroupID == id);
        }

        public async Task<List<SelectListItem>> GetGroupsAsSelectListAsync()
        {
            return await _context.StudentGroups.Select(g => new SelectListItem { Value = g.GroupID.ToString(), Text = g.GroupName }).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(StudentGroup group)
        {
            _context.Update(group);
            return Save();
        }
    }
}
