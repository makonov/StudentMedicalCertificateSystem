using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;
using Microsoft.AspNetCore.Http;

namespace StudentMedicalCertificateSystem.Controllers
{
    public class MedicalCertificatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MedicalCertificatesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var certificates = _context.MedicalCertificates.ToList();
            certificates.Reverse();
            return View(certificates);
        }

        public IActionResult Create()
        {
            MakeLists();
            return View();
        }
        
        private void MakeLists()
        {
            ViewBag.DiagnosisList = new SelectList(GetDiagnoses(), "Value", "Text");
            ViewBag.StaffList = new SelectList(GetStaff(), "Value", "Text");
            ViewBag.StatusList = new SelectList(GetClinics(), "Value", "Text");
        }

        private List<SelectListItem> GetDiagnoses()
        {
            // Здесь получаем список диагнозов из базы данных
            var diagnoses = _context.Diagnoses.ToList();
            return diagnoses.Select(d => new SelectListItem { Value = d.DiagnosisID.ToString(), Text = d.DiagnosisName }).ToList();
        }

        private List<SelectListItem> GetStaff()
        {
            // Здесь получаем список работников из базы данных
            var staff = _context.Users.Select(u => u).Where(u => u.IsAdmin).ToList();
            return staff.Select(s => new SelectListItem { Value = s.UserID.ToString(), Text = s.LastName + " " + s.FirstName + " " + s.Patronymic }).ToList();
        }

        private List<SelectListItem> GetClinics()
        {
            // Здесь получаем список работников из базы данных
            var clinics = _context.Clinics.ToList();
            return clinics.Select(c => new SelectListItem { Value = c.ClinicID.ToString(), Text = c.ClinicName }).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicalCertificates certificate, Students student, IFormFile file)
        {
            var foundStudent = _context.Students.SingleOrDefault(s =>
                s.LastName == student.LastName &&
                s.FirstName == student.FirstName &&
                s.Patronymic == student.Patronymic);

            if (foundStudent != null)
            {
                certificate.StudentID = foundStudent.StudentID;

                // Обязательная загрузка файла
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("CertificatePath", "Выберите справку для загрузки.");
                    MakeLists();
                    return View(certificate);
                }

                // Проверка формата файла
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("CertificatePath", "Выберите файл в формате jpg, jpeg или png.");
                    MakeLists();
                    return View(certificate);
                }

                // Обработка загрузки файла
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Certificates", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                certificate.CertificatePath = "/Certificates/" + fileName;

                if (certificate.IlnessDate == DateTime.Parse("01.01.0001"))
                {
                    ModelState.AddModelError("IlnessDate", "Необходимо выбрать дату!");
                    MakeLists();
                    return View(certificate);
                }

                if (certificate.RecoveryDate == DateTime.Parse("01.01.0001"))
                {
                    ModelState.AddModelError("IlnessDate", "Необходимо выбрать дату!");
                    MakeLists();
                    return View(certificate);
                }

                // Остальная логика сохранения MedicalCertificates
                certificate.CreatedAt = DateTime.Now;
                certificate.UpdatedAt = DateTime.Now;
                _context.MedicalCertificates.Add(certificate);
                _context.SaveChanges();

                var updatedCertificates = _context.MedicalCertificates
    .OrderByDescending(x => x.CertificateID)
    .Include(c => c.Student)
    .Include(c => c.User)
    .Include(c => c.Clinic)
    .Include(c => c.Diagnosis)
    .ToList();

                return View("Index", updatedCertificates);
            }
            else
            {
                ModelState.AddModelError("FirstName", "Пользователь не найден.");
                ModelState.AddModelError("LastName", "Пользователь не найден.");
                ModelState.AddModelError("Patronymic", "Пользователь не найден.");
                MakeLists();
                return View(certificate);
            }
        }


        public IActionResult Edit(int id)
        {
            var certificate = _context.MedicalCertificates.Find(id);
            MakeLists();

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MedicalCertificates certificate, IFormFile file)
        {
            if (id != certificate.CertificateID)
            {
                return NotFound();
            }

            
            var existingCertificate = _context.MedicalCertificates.Find(id);

            if (existingCertificate == null)
            {
                return NotFound();
            }

            // Проверка на наличие и формат нового файла
            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("CertificatePath", "Выберите файл в формате jpg, jpeg или png.");
                    MakeLists();
                    certificate = _context.MedicalCertificates.Find(id);
                    return View(certificate);
                }

                // Удаление старого файла
                if (!string.IsNullOrEmpty(existingCertificate.CertificatePath))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingCertificate.CertificatePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Обработка загрузки нового файла
                var newFileName = Guid.NewGuid().ToString() + fileExtension;
                var newFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Certificates", newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Присвоение пути к новому файлу в модели
                certificate.CertificatePath = "/Certificates/" + newFileName;
            }
            else
            {
                certificate.CertificatePath = existingCertificate.CertificatePath;
            }

            certificate.CreatedAt = existingCertificate.CreatedAt;
            certificate.UpdatedAt = DateTime.Now;
            _context.Entry(existingCertificate).CurrentValues.SetValues(certificate);
            _context.SaveChanges();

            var updatedCertificates = _context.MedicalCertificates.ToList();
            return View("Index", updatedCertificates);
            
        }


        [HttpPost]
        public IActionResult Filter(Students student, DateTime startOfIlness, DateTime endOfIlness)
        {
            ModelState.Remove(nameof(Students.LastName));
            ModelState.Remove(nameof(Students.FirstName));
            ModelState.Remove(nameof(Students.Patronymic));

            string lastName = student.LastName;
            string firstName = student.FirstName;
            string patronymic = student.Patronymic;

            List<MedicalCertificates> certificates = new List<MedicalCertificates>();
            List<Students> students = new List<Students>();

            if (lastName != null || firstName != null || patronymic != null)
            {
                if (lastName != null && firstName != null && patronymic != null)
                {
                    students = _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.FirstName == firstName && s.Patronymic == patronymic).ToList();
                }
                else if (lastName != null && firstName != null && patronymic == null)
                {
                    students = _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.FirstName == firstName).ToList();
                }
                else if (lastName != null && firstName == null && patronymic != null)
                {
                    students = _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName && s.Patronymic == patronymic).ToList();
                }
                else if (lastName != null && firstName == null && patronymic == null)
                {
                    students = _context.Students.Select(s => s)
                    .Where(s => s.LastName == lastName).ToList();
                }
                else if (lastName == null && firstName != null && patronymic != null)
                {
                    students = _context.Students.Select(s => s)
                    .Where(s => s.FirstName == firstName && s.Patronymic == patronymic).ToList();
                }
                else if (lastName == null && firstName == null && patronymic != null)
                {
                    students = _context.Students.Select(s => s)
                    .Where(s => s.Patronymic == patronymic).ToList();
                }
                else if (lastName == null && firstName != null && patronymic == null)
                {
                    students = _context.Students.Select(s => s)
                    .Where(s => s.FirstName == firstName).ToList();
                }

                if (students.Count() == 0)
                {
                    ModelState.AddModelError("LastName", "Студент не найден");
                    ModelState.AddModelError("FirstName", "Студент не найден");
                    ModelState.AddModelError("Patronymic", "Студент не найден");
                    return View("Index", _context.MedicalCertificates.ToList());
                }

                foreach (var s in students)
                {
                    var foundCertificates = _context.MedicalCertificates.Select(c => c).Where(c => c.StudentID == s.StudentID).ToList();
                    certificates = certificates.Union(foundCertificates).ToList();
                }

                
            }

            bool isValid = DateTime.Parse("01.01.0001") != startOfIlness && DateTime.Parse("01.01.0001") != endOfIlness;

            if (isValid && certificates.Count() != 0)
            {
                certificates = certificates.Select(c => c).Where(c => c.IlnessDate >= startOfIlness && c.RecoveryDate <= endOfIlness).ToList();
            }
            else if (isValid && certificates.Count == 0)
            {
                certificates = _context.MedicalCertificates.Select(c => c).Where(c => c.IlnessDate >= startOfIlness && c.RecoveryDate <= endOfIlness).ToList();
            }

            certificates.Reverse();

            return View("Index", certificates);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = _context.MedicalCertificates
                .FirstOrDefault(m => m.CertificateID == id);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var certificate = _context.MedicalCertificates.Find(id);

            if (certificate == null)
            {
                return NotFound();
            }

            // Удаление старого файла
            if (!string.IsNullOrEmpty(certificate.CertificatePath))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, certificate.CertificatePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            _context.MedicalCertificates.Remove(certificate);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult ImageView(string imagePath)
        {
            return View("ImageView", imagePath);
        }
    }
}
