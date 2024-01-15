using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Repository
{
    public class MedicalCertificateRepository : IMedicalCertificateRepository
    {
        private readonly ApplicationDbContext _context;
        public MedicalCertificateRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }
        public bool Add(MedicalCertificates certificate)
        {
            _context.Add(certificate);
            return Save();
        }

        public bool Delete(MedicalCertificates certificate)
        {
            _context.Remove(certificate);
            return Save();
        }

        public async Task<List<MedicalCertificates>> GetAll()
        {
            return await _context.MedicalCertificates.ToListAsync();
        }

        public async Task<List<MedicalCertificates>> GetAllSortedAndIncludedAsync()
        {
            return await  _context.MedicalCertificates
            .OrderByDescending(x => x.CertificateID)
            .Include(c => c.Student)
            .Include(c => c.User)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
            .ToListAsync();
        }

        public async Task<MedicalCertificates> GetByIdAsync(int id)
        {
            return await _context.MedicalCertificates.FirstOrDefaultAsync(c => c.CertificateID == id);
        }

        public async Task<List<MedicalCertificates>> GetAllByFullName(string lastName, string firstName, string patronymic)
        {
            return await _context.MedicalCertificates.Include(c => c.Student).Where(c => c.Student.LastName == lastName && c.Student.FirstName == firstName && c.Student.Patronymic == patronymic).ToListAsync();
        }

        public async Task<List<MedicalCertificates>> GetAllByLastAndFirstNames(string lastName, string firstName)
        {
            return await _context.MedicalCertificates.Include(c => c.Student).Where(c => c.Student.LastName == lastName && c.Student.FirstName == firstName).ToListAsync();
        }

        public async Task<List<MedicalCertificates>> GetAllByLastName(string lastName)
        {
            return await _context.MedicalCertificates.Include(c => c.Student).Where(c => c.Student.LastName == lastName).ToListAsync();
        }

        public async Task<List<MedicalCertificates>> GetAllByTimePeriod(DateTime startOfPeriod, DateTime endOfPeriod)
        {
            return await _context.MedicalCertificates.Select(c => c).Where(c => c.IlnessDate >= startOfPeriod && c.RecoveryDate <= endOfPeriod).ToListAsync();
        }

        public async Task<MedicalCertificates> GetIncludedByIdAsync(int id)
        {
            return await _context.MedicalCertificates
            .Include(c => c.Student)
            .Include(c => c.User)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
            .FirstOrDefaultAsync(c => c.CertificateID == id);
        }

        public async Task<MedicalCertificates> GetIncludedStudentByIdAsync(int id)
        {
            return await _context.MedicalCertificates.Include(c => c.Student).FirstOrDefaultAsync(c => c.CertificateID == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(MedicalCertificates certificate)
        {
            _context.Update(certificate);
            return Save();
        }

        public async Task UpdateByAnotherCertificateValues(MedicalCertificates certificateToUpdate, MedicalCertificates updatedCertificate)
        {
            _context.Entry(certificateToUpdate).CurrentValues.SetValues(updatedCertificate);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MedicalCertificates>> GetAllByStudentId(int studentId)
        {
            return await _context.MedicalCertificates.Select(c => c).Where(c => c.StudentID == studentId).ToListAsync();
        }
    }
}
