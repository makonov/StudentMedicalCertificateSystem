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

        public bool Add(Students student)
        {
            _context.Add(student);
            return Save();
        }

        public bool Delete(Students student)
        {
            _context.Remove(student);
            return Save();
        }

        public async Task<List<Students>> GetAll()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Students> GetDefaultByFullName(string lastName, string firstName, string patronymic)
        {
            return await _context.Students.SingleOrDefaultAsync(s => s.LastName == lastName && s.FirstName == firstName && s.Patronymic == patronymic);
        }

        public async Task<Students> GetByIdAsync(int id)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.StudentID == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Students student)
        {
            _context.Update(student);
            return Save();
        }

        public async Task<List<Students>> GetAllByFullName(string lastName, string firstName, string patronymic)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.FirstName == firstName && s.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<Students>> GetAllByLastAndFirstNames(string lastName, string firstName)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.FirstName == firstName).ToListAsync();
        }

        public async Task<List<Students>> GetAllByLastNameAndPatronymic(string lastName, string patronymic)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<Students>> GetAllByLastName(string lastName)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName).ToListAsync();
        }

        public async Task<List<Students>> GetAllByFirstNameAndPatronymic(string firstName, string patronymic)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.FirstName == firstName && s.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<Students>> GetAllByPatronymic(string patronymic)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<Students>> GetAllByFirstName(string firstName)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.FirstName == firstName).ToListAsync();
        }
    }
}
