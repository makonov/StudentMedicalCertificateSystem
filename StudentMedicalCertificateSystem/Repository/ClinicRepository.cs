using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Repository
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly ApplicationDbContext _context;

        public ClinicRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }

        public bool Add(Clinic clinic)
        {
            _context.Add(clinic);
            return Save();
        }

        public bool Delete(Clinic clinic)
        {
            _context.Remove(clinic);
            return Save();
        }

        public async Task<List<Clinic>> GetAllSortedAsync()
        {
            return await _context.Clinics.OrderBy(c => c.ClinicName).ToListAsync();
        }

        public async Task<Clinic> GetByIdAsync(int id)
        {
            return await _context.Clinics.FirstOrDefaultAsync(c => c.ClinicID == id);
        }

        public async Task<List<SelectListItem>> GetClinicsAsSelectListAsync()
        {
            return await _context.Clinics.Select(c => new SelectListItem { Value = c.ClinicID.ToString(), Text = c.ClinicName }).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Clinic clinic)
        {
            _context.Update(clinic);
            return Save();
        }
    }
}
