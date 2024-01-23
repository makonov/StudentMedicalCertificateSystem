﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;
using Microsoft.AspNetCore.Http;
using StudentMedicalCertificateSystem.Interfaces;
using System.Collections;
using StudentMedicalCertificateSystem.Repository;
using Microsoft.AspNetCore.Authorization;
using StudentMedicalCertificateSystem.ViewModels;

namespace StudentMedicalCertificateSystem.Controllers
{
    
    public class MedicalCertificateController : Controller
    {
        private readonly IMedicalCertificateRepository _certificateRepository;
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IPhotoService _photoService;

        public MedicalCertificateController(IMedicalCertificateRepository certificateRepository, 
            IDiagnosisRepository diagnosisRepository, 
            IUserRepository userRepository,
            IClinicRepository clinicRepository,
            IStudentRepository studentRepository, 
            IPhotoService photoService)
        {
            _certificateRepository = certificateRepository;
            _diagnosisRepository = diagnosisRepository;
            _userRepository = userRepository;
            _clinicRepository = clinicRepository;
            _studentRepository = studentRepository;
            _photoService = photoService;
        }

        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Index()
        {
            var certificates = await _certificateRepository.GetAllSortedAndIncludedAsync();
            return View(certificates);
        }

        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Create()
        {
            await MakeLists();
            var students = await _studentRepository.GetAllIncludedGroupAsync();
            
            var studentFullNamesWithGroups = new List<(string, string)>();
            foreach (var student in students)
            {
                string fullName = $"{student.LastName} {student.FirstName} {student.Patronymic}";
                studentFullNamesWithGroups.Add((fullName, student.Group.GroupName));
            }
            var medicalCertificateViewModel = new CreateMedicalCertificateViewModel
            {
                AllStudentFullNamesWithGroups = studentFullNamesWithGroups
            };

            return View(medicalCertificateViewModel);
        }
        
        private async Task MakeLists()
        {
            ViewBag.DiagnosisList = new SelectList(await GetDiagnoses(), "Value", "Text");
            ViewBag.ClinicList = new SelectList(await GetClinics(), "Value", "Text");
            ViewBag.StudentList = new SelectList(await GetStudentFullNamesWithGroups(), "Value", "Text");
        }

        private async Task<List<SelectListItem>> GetDiagnoses()
        {
            // Здесь получаем список диагнозов из базы данных
            return await _diagnosisRepository.GetDiagnosesAsSelectList();
        }

        private async Task<List<SelectListItem>> GetClinics()
        {
            // Здесь получаем список работников из базы данных
            return await _clinicRepository.GetClinicsAsSelectList();
        }

        private async Task<List<SelectListItem>> GetStudentFullNamesWithGroups()
        {
            // Здесь получаем список работников из базы данных
            //var students = await _studentRepository.GetAllIncludedGroupAsync();

            //var studentFullNamesWithGroups = new List<(string, string)>();
            //foreach (var student in students)
            //{
            //    string fullName = $"{student.LastName} {student.FirstName} {student.Patronymic}";
            //    studentFullNamesWithGroups.Add((fullName, student.Group.GroupName));
            //}
            return await _studentRepository.GetStudentFullNamesWithGroupsAsSelectedList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Create(CreateMedicalCertificateViewModel certificateViewModel, IFormFile file)
        {
            // Проверка, выбран ли студент
            if (certificateViewModel.FullName == null)
            {
                ModelState.AddModelError("FullName", "Выберите студента");
                await MakeLists();
                return View(certificateViewModel);
            }

            // Поиск выбранного студента
            string[] fullName = certificateViewModel.FullName.Split();
            string lastName = fullName[0];
            string firstName = fullName[1];
            string patronymic = fullName[2];
            Student foundStudent = await _studentRepository.GetDefaultByFullName(lastName, firstName, patronymic);

            // Если студент найден
            if (foundStudent != null)
            {
                certificateViewModel.StudentID = foundStudent.StudentID;

                var uploadResult = await _photoService.UploadPhotoAsync(file, "Certificates");

                if (!uploadResult.IsUploadedAndExtensionValid)
                {
                    ModelState.AddModelError("CertificatePath", "Выберите файл в формате jpg, jpeg, png или pdf.");
                    await MakeLists();
                    return View(certificateViewModel);
                }

                certificateViewModel.CertificatePath = "/Certificates/" + uploadResult.FileName;

                if (certificateViewModel.IlnessDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("IlnessDate", "Необходимо выбрать дату!");
                    await MakeLists();
                    return View(certificateViewModel);
                }

                if (certificateViewModel.RecoveryDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("RecoveryDate", "Необходимо выбрать дату!");
                    await MakeLists();
                    return View(certificateViewModel);
                }

                var certificate = new MedicalCertificate
                {
                    StudentID = certificateViewModel.StudentID,
                    ClinicID = certificateViewModel.ClinicID,
                    DiagnosisID = certificateViewModel.DiagnosisID,
                    CertificatePath = certificateViewModel.CertificatePath,
                    IlnessDate = certificateViewModel.IlnessDate.HasValue ? certificateViewModel.IlnessDate.Value : default(DateTime),
                    RecoveryDate = certificateViewModel.RecoveryDate.HasValue ? certificateViewModel.RecoveryDate.Value : default(DateTime),
                    Answer = certificateViewModel.Answer,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Остальная логика сохранения MedicalCertificates
                _certificateRepository.Add(certificate);
                _certificateRepository.Save();

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("FullName", "Пользователь не найден.");
                await MakeLists();
                return View(certificateViewModel);
            }
        }

        [Authorize(Roles = "admin, user")]
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
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Edit(int id, MedicalCertificate certificate, IFormFile file)
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
                var replacementResult = await _photoService.ReplacePhotoAsync(file, "Certificates", existingCertificate.CertificatePath);

                if (!replacementResult.IsReplacementSuccess)
                {
                    ModelState.AddModelError("CertificatePath", "Выберите файл в формате jpg, jpeg или png.");
                    await MakeLists();
                    certificate = await _certificateRepository.GetIncludedStudentByIdAsync(id);
                    return View(certificate);
                }

                // Присвоение пути к новому файлу в модели
                certificate.CertificatePath = "/Certificates/" + replacementResult.NewFileName;
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
        public async Task<IActionResult> Filter(Student student, DateTime startOfIlness, DateTime endOfIlness)
        {
            ModelState.Remove(nameof(Student.LastName));
            ModelState.Remove(nameof(Student.FirstName));
            ModelState.Remove(nameof(Student.Patronymic));

            string lastName = student.LastName;
            string firstName = student.FirstName;
            string patronymic = student.Patronymic;

            List<MedicalCertificate> certificates = new List<MedicalCertificate>();
            List<Student> students = new List<Student>();

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

            return View("Index", await _certificateRepository.GetSortedAndIncludedFromList(certificates));
        }

        [Authorize(Roles = "admin, user")]
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
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificate = await _certificateRepository.GetByIdAsync(id);

            if (certificate == null)
            {
                return NotFound();
            }

            // Удаление файла с использованием PhotoService
            await _photoService.DeletePhotoAsync(certificate.CertificatePath);

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
