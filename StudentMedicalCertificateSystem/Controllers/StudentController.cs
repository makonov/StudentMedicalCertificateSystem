using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;

namespace StudentMedicalCertificateSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public IActionResult Index()
        {
            var students = _studentRepository.GetAllSortedAndIncludedAsync();
            return View(students);
        }
    }
}
