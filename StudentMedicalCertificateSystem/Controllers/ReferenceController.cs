using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Interfaces;

namespace StudentMedicalCertificateSystem.Controllers
{
    public class ReferenceController : Controller
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly IEducationalOfficeRepository _officeRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IStudentGroupRepository _groupRepository;
        public ReferenceController(IClinicRepository clinicRepository, 
            IDiagnosisRepository diagnosisRepository,
            IEducationalOfficeRepository officeRepository,
            IStudentRepository studentRepository,
            IStudentGroupRepository groupRepository)
        {
            _clinicRepository = clinicRepository;
            _diagnosisRepository = diagnosisRepository;
            _officeRepository = officeRepository;
            _studentRepository = studentRepository;
            _groupRepository = groupRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Show

        public IActionResult Clinic()
        {
            var clinics = _clinicRepository.GetAll();
            return View(clinics);
        }

        public IActionResult Diagnosis()
        {
            var diagnoses = _diagnosisRepository.GetAll();
            return View(diagnoses);
        }

        public IActionResult Office()
        {
            var offices = _officeRepository.GetAll();
            return View(offices);
        }

        public IActionResult Group()
        {
            var groups = _groupRepository.GetAll();
            return View(groups);
        }

        public IActionResult Student()
        {
            var students = _studentRepository.GetAllSortedAndIncludedAsync();
            return View(students);
        }

        #endregion
    }
}
