using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }

        public bool Add(Student student)
        {
            _context.Add(student);
            return Save();
        }

        public bool Delete(Student student)
        {
            _context.Remove(student);
            return Save();
        }

        public async Task<List<Student>> GetAll()
        {
            return await _context.Students.ToListAsync();
        }


        public async Task<List<Student>> GetAllSortedAndIncludedAsync()
        {
            return await _context.Students
                .OrderByDescending(s => s.StudentID)
                .Include(s => s.Group)
                .Include(s => s.Office)
                .ToListAsync();
        }

        public async Task<Student> GetDefaultByFullName(string lastName, string firstName, string patronymic)
        {
            return await _context.Students.SingleOrDefaultAsync(s => s.LastName == lastName && s.FirstName == firstName && s.Patronymic == patronymic);
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.StudentID == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Student student)
        {
            _context.Update(student);
            return Save();
        }

        public async Task<List<Student>> GetAllIncludedGroupAsync()
        {
            return await _context.Students.Include(s => s.Group).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetStudentFullNamesWithGroupsAsSelectedList()
        {
            return await _context.Students.Include(s => s.Group).Select(s => new SelectListItem { Value = $"{s.LastName} {s.FirstName} {s.Patronymic}", Text = $"{s.LastName} {s.FirstName} {s.Patronymic} - {s.Group.GroupName}" }).ToListAsync();
        }
    }
}
