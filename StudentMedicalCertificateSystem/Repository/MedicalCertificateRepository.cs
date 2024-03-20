using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using System.Security.Claims;

namespace StudentMedicalCertificateSystem.Repository
{
    
    public class MedicalCertificateRepository : IMedicalCertificateRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalCertificateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(MedicalCertificate certificate)
        {
            _context.Add(certificate);
            return Save();
        }

        public bool Delete(MedicalCertificate certificate)
        {
            _context.Remove(certificate);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(MedicalCertificate certificate)
        {
            _context.Update(certificate);
            return Save();
        }

        public async Task<List<MedicalCertificate>> GetAllSortedAndIncludedAsync()
        {
            return await  _context.MedicalCertificates
            .OrderByDescending(x => x.CertificateID)
            .Include(c => c.Student)
            .Include(c => c.Student.Group)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
            .Include(c => c.User)
            .ToListAsync();
        }

        public async Task<MedicalCertificate> GetByIdAsync(int id)
        {
            return await _context.MedicalCertificates.FirstOrDefaultAsync(c => c.CertificateID == id);
        }

        public async Task<List<MedicalCertificate>> GetAllByTimePeriodAsync(DateTime startOfPeriod, DateTime endOfPeriod)
        {
            return await _context.MedicalCertificates
                .OrderByDescending(c => c.CertificateID)
                .Include(c => c.Student)
                .Include(c => c.Student.Group)
                .Include (c => c.Clinic)
                .Include(c => c.Diagnosis)
                .Include(c => c.User)
                .Where(c => c.IllnessDate >= startOfPeriod && c.IllnessDate <= endOfPeriod)
                .ToListAsync();
        }

        public async Task<MedicalCertificate> GetIncludedByIdAsync(int id)
        {
            return await _context.MedicalCertificates
            .Include(c => c.Student)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.CertificateID == id);
        }

        public async Task<List<MedicalCertificate>> GetAllSortedAndIncludedByStudentIdAsync(int id)
        {
            return await _context.MedicalCertificates
             .OrderByDescending(x => x.CertificateID)
             .Include(c => c.Student)
             .Include(c => c.Student.Group)
             .Include(c => c.Clinic)
             .Include(c => c.Diagnosis)
             .Include(c => c.User)
             .Where(c => c.StudentID == id)
             .ToListAsync();
        }

        

        public async Task<int> CountAsync()
        {
            return await _context.MedicalCertificates.Include(c => c.Student).CountAsync();
        }

        public async Task<List<MedicalCertificate>> GetPagedCertificatesAsync(int page, int pageSize)
        {
            return await _context.MedicalCertificates
            .OrderByDescending(c => c.CertificateID)
            .Include(c => c.Student)
            .Include(c => c.Student.Group)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
            .Include(c => c.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }

        public async Task<List<MedicalCertificate>> GetAllIncludedAsync()
        {
            return await _context.MedicalCertificates
            .Include(c => c.Student)
            .Include(c => c.Student.Group)
            .Include(c => c.Student.Group.Program)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
            .Include(c => c.User)
            .ToListAsync();
        }

        public async Task<List<MedicalCertificate>> GetAllByProgramIdAsync(int id)
        {
            return await _context.MedicalCertificates
                .OrderByDescending(c => c.CertificateID)
                .Include(c => c.Student)
                .Include(c => c.Student.Group)
                .Include(c => c.Student.Group.Program)
                .Include(c => c.Clinic)
                .Include(c => c.Diagnosis)
                .Include(c => c.User)
                .Where(c => c.Student.Group.ProgramID  == id)
                .ToListAsync();
        }
    }
}
