﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;
using Microsoft.AspNetCore.Http;
using StudentMedicalCertificateSystem.Interfaces;
using System.Collections;

namespace StudentMedicalCertificateSystem.Controllers
{
    public class MedicalCertificatesController : Controller
    {
        private readonly IMedicalCertificateRepository _certificateRepository;
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MedicalCertificatesController(IMedicalCertificateRepository certificateRepository, IDiagnosisRepository diagnosisRepository, IUserRepository userRepository,IClinicRepository clinicRepository, IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment)
        {
            _certificateRepository = certificateRepository;
            _diagnosisRepository = diagnosisRepository;
            _userRepository = userRepository;
            _clinicRepository = clinicRepository;
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var certificates = await _certificateRepository.GetAllSortedAndIncludedAsync();
            return View(certificates);
        }

        public async Task<IActionResult> Create()
        {
            await MakeLists();
            return View();
        }
        
        private async Task MakeLists()
        {
            
            ViewBag.StaffList = new SelectList(await GetStaff(), "Value", "Text");
            ViewBag.DiagnosisList = new SelectList(await GetDiagnoses(), "Value", "Text");
            ViewBag.StatusList = new SelectList(await GetClinics(), "Value", "Text");
        }

        private async Task<List<SelectListItem>> GetDiagnoses()
        {
            // Здесь получаем список диагнозов из базы данных
            return await _diagnosisRepository.GetDiagnosesAsSelectList();
        }

        private async Task<List<SelectListItem>> GetStaff()
        {
            // Здесь получаем список работников из базы данных
            return await _userRepository.GetUsersAsSelectList();
        }

        private async Task<List<SelectListItem>> GetClinics()
        {
            // Здесь получаем список работников из базы данных
            return await _clinicRepository.GetClinicsAsSelectList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalCertificates certificate, Students student, IFormFile file)
        {
            Students foundStudent = await _studentRepository.GetDefaultByFullName(student.LastName, student.FirstName, student.Patronymic);

            if (foundStudent != null)
            {
                certificate.StudentID = foundStudent.StudentID;

                // Обязательная загрузка файла
                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("CertificatePath", "Выберите справку для загрузки.");
                    await MakeLists();
                    return View(certificate);
                }

                // Проверка формата файла
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("CertificatePath", "Выберите файл в формате jpg, jpeg или png.");
                    await MakeLists();
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
                    await MakeLists();
                    return View(certificate);
                }

                if (certificate.RecoveryDate == DateTime.Parse("01.01.0001"))
                {
                    ModelState.AddModelError("IlnessDate", "Необходимо выбрать дату!");
                    await MakeLists();
                    return View(certificate);
                }

                // Остальная логика сохранения MedicalCertificates
                certificate.CreatedAt = DateTime.Now;
                certificate.UpdatedAt = DateTime.Now;
                _certificateRepository.Add(certificate);
                _certificateRepository.Save();

                var updatedCertificates = await _certificateRepository.GetAllSortedAndIncludedAsync();

                return View("Index", updatedCertificates);
            }
            else
            {
                ModelState.AddModelError("FirstName", "Пользователь не найден.");
                ModelState.AddModelError("LastName", "Пользователь не найден.");
                ModelState.AddModelError("Patronymic", "Пользователь не найден.");
                await MakeLists();
                return View(certificate);
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            var certificate = await _certificateRepository.GetIncludedStudentByIdAsync(id);
            await MakeLists();

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalCertificates certificate, IFormFile file)
        {
            if (id != certificate.CertificateID)
            {
                return NotFound();
            }


            var existingCertificate = await _certificateRepository.GetByIdAsync(id);

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
                    await MakeLists();
                    certificate = await _certificateRepository.GetIncludedStudentByIdAsync(id);
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
            await _certificateRepository.UpdateByAnotherCertificateValues(existingCertificate, certificate);
            _certificateRepository.Save();

            var updatedCertificates = await _certificateRepository.GetAllSortedAndIncludedAsync();
            return View("Index", updatedCertificates);
        }


        [HttpPost]
        public async Task<IActionResult> Filter(Students student, DateTime startOfIlness, DateTime endOfIlness)
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
                    students = await _studentRepository.GetAllByFullName(lastName, firstName, patronymic);
                }
                else if (lastName != null && firstName != null && patronymic == null)
                {
                    students = await _studentRepository.GetAllByLastAndFirstNames(lastName, firstName);
                }
                else if (lastName != null && firstName == null && patronymic != null)
                {
                    students = await _studentRepository.GetAllByLastNameAndPatronymic(lastName, patronymic);
                }
                else if (lastName != null && firstName == null && patronymic == null)
                {
                    students = await _studentRepository.GetAllByLastName(lastName);
                }
                else if (lastName == null && firstName != null && patronymic != null)
                {
                    students = await _studentRepository.GetAllByFirstNameAndPatronymic(firstName, patronymic);
                }
                else if (lastName == null && firstName == null && patronymic != null)
                {
                    students = await _studentRepository.GetAllByPatronymic(patronymic);
                }
                else if (lastName == null && firstName != null && patronymic == null)
                {
                    students = await _studentRepository.GetAllByFirstName(firstName);
                }

                if (students.Count() == 0)
                {
                    ModelState.AddModelError("LastName", "Студент не найден");
                    ModelState.AddModelError("FirstName", "Студент не найден");
                    ModelState.AddModelError("Patronymic", "Студент не найден");
                    return View("Index", await _certificateRepository.GetAllSortedAndIncludedAsync());
                }

                foreach (var s in students)
                {
                    var foundCertificates = await _certificateRepository.GetAllByStudentId(s.StudentID);
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
                certificates = await _certificateRepository.GetAllByTimePeriod(startOfIlness, endOfIlness);
            }

            certificates.Reverse();

            return View("Index", certificates);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _certificateRepository.GetIncludedByIdAsync(id);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificate = await _certificateRepository.GetByIdAsync(id);

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

            _certificateRepository.Delete(certificate);
            _certificateRepository.Save();

            return RedirectToAction("Index");
        }

        public IActionResult ImageView(string imagePath)
        {
            return View("ImageView", imagePath);
        }
    }
}
