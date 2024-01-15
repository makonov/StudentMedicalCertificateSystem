using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Repository
{
    public class DiagnosisRepository : IDiagnosisRepository
    {
        private readonly ApplicationDbContext _context;
        public DiagnosisRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(Diagnoses diagnosis)
        {
            _context.Add(diagnosis);
            return Save();
        }

        public bool Delete(Diagnoses diagnosis)
        {
            _context.Remove(diagnosis);
            return Save();
        }

        public async Task<List<Diagnoses>> GetAll()
        {
            return await _context.Diagnoses.ToListAsync();
        }

        public async Task<Diagnoses> GetByIdAsync(int id)
        {
            return await _context.Diagnoses.FirstOrDefaultAsync(d => d.DiagnosisID == id);
        }

        public async Task<List<SelectListItem>> GetDiagnosesAsSelectList()
        {
            return await _context.Diagnoses.Select(d => new SelectListItem { Value = d.DiagnosisID.ToString(), Text = d.DiagnosisName }).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Diagnoses diagnosis)
        {
            _context.Update(diagnosis);
            return Save();
        }
    }
}
