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

        public bool Add(Clinics clinic)
        {
            _context.Add(clinic);
            return Save();
        }

        public bool Delete(Clinics clinic)
        {
            _context.Remove(clinic);
            return Save();
        }

        public async Task<List<Clinics>> GetAll()
        {
            return await _context.Clinics.ToListAsync();
        }

        public async Task<Clinics> GetByIdAsync(int id)
        {
            return await _context.Clinics.FirstOrDefaultAsync(c => c.ClinicID == id);
        }

        public async Task<List<SelectListItem>> GetClinicsAsSelectList()
        {
            return await _context.Clinics.Select(c => new SelectListItem { Value = c.ClinicID.ToString(), Text = c.ClinicName }).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Clinics clinic)
        {
            _context.Update(clinic);
            return Save();
        }
    }
}
