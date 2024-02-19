using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Repository
{
    public class EducationalOfficeRepository : IEducationalOfficeRepository
    {
        private readonly ApplicationDbContext _context;
        public EducationalOfficeRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }
        public bool Add(EducationalOffice office)
        {
            _context.Add(office);
            return Save();
        }

        public bool Delete(EducationalOffice office)
        {
            _context.Remove(office);
            return Save();
        }

        public async Task<List<EducationalOffice>> GetAllSorted()
        {
            return await _context.EducationalOffices.OrderBy(o => o.OfficeName).ToListAsync();
        }

        public async Task<EducationalOffice> GetByIdAsync(int id)
        {
            return await _context.EducationalOffices.FirstOrDefaultAsync(o => o.OfficeID == id);
        }

        public async Task<List<SelectListItem>> GetOfficesAsSelectList()
        {
            return await _context.EducationalOffices.Select(o => new SelectListItem { Value = o.OfficeID.ToString(), Text = o.OfficeName }).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(EducationalOffice office)
        {
            _context.Update(office);
            return Save();
        }
    }
}
