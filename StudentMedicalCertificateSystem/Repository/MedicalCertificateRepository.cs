using Microsoft.AspNetCore.Authorization;
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

        public async Task<List<MedicalCertificate>> GetAll()
        {
            return await _context.MedicalCertificates.ToListAsync();
        }

        public async Task<List<MedicalCertificate>> GetAllSortedAndIncludedAsync()
        {
            return await  _context.MedicalCertificates
            .OrderByDescending(x => x.CertificateID)
            .Include(c => c.Student)
            .Include(c => c.Student.Group)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
            .ToListAsync();
        }

        public async Task<MedicalCertificate> GetByIdAsync(int id)
        {
            return await _context.MedicalCertificates.FirstOrDefaultAsync(c => c.CertificateID == id);
        }

        public async Task<List<MedicalCertificate>> GetAllByTimePeriod(DateTime startOfPeriod, DateTime endOfPeriod)
        {
            return await _context.MedicalCertificates
                .OrderByDescending(c => c.CertificateID)
                .Include(c => c.Student)
                .Include(c => c.Student.Group)
                .Include (c => c.Clinic)
                .Include(c => c.Diagnosis)
                .Where(c => c.IlnessDate >= startOfPeriod && c.RecoveryDate <= endOfPeriod)
                .ToListAsync();
        }

        public async Task<MedicalCertificate> GetIncludedByIdAsync(int id)
        {
            return await _context.MedicalCertificates
            .Include(c => c.Student)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
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
             .Where(c => c.StudentID == id)
             .ToListAsync();
        }

        public async Task<MedicalCertificate> GetIncludedStudentByIdAsync(int id)
        {
            return await _context.MedicalCertificates.Include(c => c.Student).FirstOrDefaultAsync(c => c.CertificateID == id);
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

        public async Task UpdateByAnotherCertificateValues(MedicalCertificate certificateToUpdate, MedicalCertificate updatedCertificate)
        {
            _context.Entry(certificateToUpdate).CurrentValues.SetValues(updatedCertificate);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MedicalCertificate>> GetAllByStudentId(int studentId)
        {
            return await _context.MedicalCertificates.Where(c => c.StudentID == studentId).ToListAsync();
        }

        public async Task<List<MedicalCertificate>> GetSortedAndIncludedFromList(List<MedicalCertificate> list)
        {
            var certificateIds = list.Select(c => c.CertificateID).ToList();

            return await _context.MedicalCertificates
                .Where(c => certificateIds.Contains(c.CertificateID))
                .OrderByDescending(x => x.CertificateID)
                .Include(c => c.Student)
                .Include(c => c.Student.Group)
                .Include(c => c.Clinic)
                .Include(c => c.Diagnosis)
                .ToListAsync();
        }

        public async Task<int> Count()
        {
            return await _context.MedicalCertificates.CountAsync();
        }

        public async Task<List<MedicalCertificate>> GetPagedCertificates(int page, int pageSize)
        {
            return await _context.MedicalCertificates
            .OrderByDescending(c => c.CertificateID)
            .Include(c => c.Student)
            .Include(c => c.Student.Group)
            .Include(c => c.Clinic)
            .Include(c => c.Diagnosis)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }

        public async Task<List<MedicalCertificate>> GetPagedCertificatesFromList(List<MedicalCertificate> certificates, int page, int pageSize)
        {
            var certificateIds = certificates.Select(c => c.CertificateID).ToList();

            var pagedCertificates = _context.MedicalCertificates
                .Where(c => certificateIds.Contains(c.CertificateID))
                .OrderByDescending(c => c.CertificateID)
                .Include(c => c.Student)
                .Include(c => c.Student.Group)
                .Include(c => c.Clinic)
                .Include(c => c.Diagnosis)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return pagedCertificates;
        }

    }
}
