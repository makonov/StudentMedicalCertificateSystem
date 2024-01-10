using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Data;

namespace StudentMedicalCertificateSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var students = _context.Students.ToList();
            return View(students);
        }
    }
}
