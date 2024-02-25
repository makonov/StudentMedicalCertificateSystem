using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;

namespace StudentMedicalCertificateSystem.Repository
{
    public class EducationalProgramRepository : IEducationalProgramRepository
    {
        private readonly ApplicationDbContext _context;
        public EducationalProgramRepository(ApplicationDbContext context) 
        { 
            _context = context;
        }
        public bool Add(EducationalProgram program)
        {
            _context.Add(program);
            return Save();
        }

        public bool Delete(EducationalProgram program)
        {
            _context.Remove(program);
            return Save();
        }

        public async Task<List<EducationalProgram>> GetAllSorted()
        {
            return await _context.EducationalPrograms.OrderBy(p => p.ProgramName).ToListAsync();
        }

        public async Task<EducationalProgram> GetByIdAsync(int id)
        {
            return await _context.EducationalPrograms.FirstOrDefaultAsync(p => p.ProgramID == id);
        }

        public async Task<List<SelectListItem>> GetProgramsAsSelectList()
        {
            return await _context.EducationalPrograms.Select(p => new SelectListItem { Value = p.ProgramID.ToString(), Text = p.ProgramName }).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(EducationalProgram program)
        {
            _context.Update(program);
            return Save();
        }
    }
}
