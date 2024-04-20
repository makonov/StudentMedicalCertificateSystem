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

        public bool Add(Diagnosis diagnosis)
        {
            _context.Add(diagnosis);
            return Save();
        }

        public bool Delete(Diagnosis diagnosis)
        {
            _context.Remove(diagnosis);
            return Save();
        }

        public async Task<List<Diagnosis>> GetAllSortedAsync()
        {
            return await _context.Diagnoses
                .OrderBy(d => d.DiagnosisName)
                .ToListAsync();
        }

        public async Task<Diagnosis> GetByIdAsync(int id)
        {
            return await _context.Diagnoses.FirstOrDefaultAsync(d => d.DiagnosisID == id);
        }

        public async Task<List<SelectListItem>> GetDiagnosesAsSelectListAsync()
        {
            return await _context.Diagnoses.Select(d => new SelectListItem { Value = d.DiagnosisID.ToString(), Text = $"{d.DiagnosisName} {d.Code}" }).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Diagnosis diagnosis)
        {
            _context.Update(diagnosis);
            return Save();
        }

        public bool DiagnosisExistsByName(string name)
        {
            return _context.Diagnoses.Any(d => d.DiagnosisName == name);
        }

        public bool DiagnosisExistsByCode(string code)
        {
            return _context.Diagnoses.Any(d => d.Code == code);
        }
    }
}
