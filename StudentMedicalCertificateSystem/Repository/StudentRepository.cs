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

        public async Task<List<Student>> GetAllByFullName(string lastName, string firstName, string patronymic)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.FirstName == firstName && s.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<Student>> GetAllByLastAndFirstNames(string lastName, string firstName)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.FirstName == firstName).ToListAsync();
        }

        public async Task<List<Student>> GetAllByLastNameAndPatronymic(string lastName, string patronymic)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<Student>> GetAllByLastName(string lastName)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName).ToListAsync();
        }

        public async Task<List<Student>> GetAllByFirstNameAndPatronymic(string firstName, string patronymic)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.FirstName == firstName && s.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<Student>> GetAllByPatronymic(string patronymic)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<Student>> GetAllByFirstName(string firstName)
        {
            return await _context.Students.Select(s => s)
                    .Where(s => s.FirstName == firstName).ToListAsync();
        }
    }
}
