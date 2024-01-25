using Microsoft.AspNetCore.Hosting;
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
            ViewBag.StudentList = new SelectList(await GetStudentFullNamesWithGroups(), "Value", "Text");
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
            // Здесь получаем список студентов из базы данных
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
            var certificate = await _certificateRepository.GetIncludedByIdAsync(id);

            var certificateViewModel = new EditMedicalCertificateViewModel
            {
                CertificateID = certificate.CertificateID,
                StudentID = certificate.StudentID,
                Student = certificate.Student,
                ClinicID = certificate.ClinicID,
                DiagnosisID = certificate.DiagnosisID,
                CertificatePath = certificate.CertificatePath,
                IlnessDate = certificate.IlnessDate,
                RecoveryDate = certificate.RecoveryDate,
                Answer = certificate.Answer,
                CreatedAt = certificate.CreatedAt
            };

            await MakeLists();

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificateViewModel);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Edit(int id, EditMedicalCertificateViewModel certificateViewModel, IFormFile file)
        {
            

            var existingCertificate = await _certificateRepository.GetIncludedByIdAsync(id);

            if (existingCertificate == null)
            {
                ModelState.AddModelError("", "Произошла ошибка при попытке обновить справку");
                await MakeLists();
                certificateViewModel.Student = existingCertificate.Student;
                return View(certificateViewModel);
            }

            // Проверка на наличие и формат нового файла
            if (file != null)
            {
                var replacementResult = await _photoService.ReplacePhotoAsync(file, "Certificates", existingCertificate.CertificatePath);

                if (!replacementResult.IsReplacementSuccess)
                {
                    ModelState.AddModelError("CertificatePath", "Выберите файл в формате jpg, jpeg, png или pdf");
                    await MakeLists();
                    certificateViewModel.Student = existingCertificate.Student;
                    return View(certificateViewModel);
                }

                // Присвоение пути к новому файлу в модели
                certificateViewModel.CertificatePath = "/Certificates/" + replacementResult.NewFileName;
            }
            else
            {
                certificateViewModel.CertificatePath = existingCertificate.CertificatePath;
            }

            if (certificateViewModel.IlnessDate == DateTime.MinValue)
            {
                ModelState.AddModelError("IlnessDate", "Необходимо выбрать дату!");
                await MakeLists();
                certificateViewModel.Student = existingCertificate.Student;
                return View(certificateViewModel);
            }

            if (certificateViewModel.RecoveryDate == DateTime.MinValue)
            {
                ModelState.AddModelError("RecoveryDate", "Необходимо выбрать дату!");
                await MakeLists();
                certificateViewModel.Student = existingCertificate.Student;
                return View(certificateViewModel);
            }

            var certificate = new MedicalCertificate
            {
                CertificateID = certificateViewModel.CertificateID,
                StudentID = certificateViewModel.StudentID,
                ClinicID = certificateViewModel.ClinicID,
                DiagnosisID = certificateViewModel.DiagnosisID,
                CertificatePath = certificateViewModel.CertificatePath,
                IlnessDate = certificateViewModel.IlnessDate.HasValue ? certificateViewModel.IlnessDate.Value : default(DateTime),
                RecoveryDate = certificateViewModel.RecoveryDate.HasValue ? certificateViewModel.RecoveryDate.Value : default(DateTime),
                Answer = certificateViewModel.Answer,
                CreatedAt = existingCertificate.CreatedAt,
                UpdatedAt = DateTime.Now

            };

            await _certificateRepository.UpdateByAnotherCertificateValues(existingCertificate, certificate);
            _certificateRepository.Save();
            certificateViewModel.Student = existingCertificate.Student;
            return RedirectToAction("Index");   
        }


        [HttpPost]
        public async Task<IActionResult> Filter(FilterCertificatesViewModel filterViewModel)
        {
            ModelState.Remove(nameof(filterViewModel.FullName));
            var certificates = new List<MedicalCertificate>();          

            if (filterViewModel.FullName != null)
            {
                // Поиск выбранного студента
                string[] fullName = filterViewModel.FullName.Split();
                string lastName = fullName[0];
                string firstName = fullName[1];
                string patronymic = fullName[2];
                Student foundStudent = await _studentRepository.GetDefaultByFullName(lastName, firstName, patronymic);
                certificates = await _certificateRepository.GetAllSortedAndIncludedByStudentIdAsync(foundStudent.StudentID);
            }

            var ilnessDate = filterViewModel.IlnessDate.HasValue ? filterViewModel.IlnessDate.Value : default(DateTime);
            var recoveryDate = filterViewModel.RecoveryDate.HasValue ? filterViewModel.RecoveryDate.Value : default(DateTime);
            bool isValid = ilnessDate != DateTime.MinValue && recoveryDate != DateTime.MinValue;

            if (isValid && certificates.Count() != 0)
            {
                certificates = certificates.Select(c => c)
                    .Where(c => c.IlnessDate >= filterViewModel.IlnessDate
                    && c.RecoveryDate <= filterViewModel.RecoveryDate).ToList();
            }
            else if (isValid && certificates.Count == 0)
            {
                certificates = await _certificateRepository.GetAllByTimePeriod(ilnessDate, recoveryDate);
            }

            ViewBag.StudentList = new SelectList(await GetStudentFullNamesWithGroups(), "Value", "Text");
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

        [Authorize(Roles = "admin, user, guest")]
        public IActionResult ImageView(string imagePath)
        {
            return View("ImageView", imagePath);
        }
    }
}
