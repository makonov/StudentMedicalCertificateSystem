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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DocumentFormat.OpenXml.EMMA;

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
        private const int PageSize = 10;
        private static List<MedicalCertificate> filtered;

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

        public async Task<IActionResult> Index(int page = 1)
        {
            var totalCount = await _certificateRepository.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            var certificates = await _certificateRepository.GetPagedCertificates(page, PageSize);
            ViewBag.StudentList = new SelectList(await GetStudentFullNamesWithGroups(), "Value", "Text");

            var viewModel = new ShowMedicalCertificateViewModel
            {
                Certificates = certificates,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = totalCount,
                    TotalPages = totalPages
                }
            };
            return View(viewModel);
        }

        [Authorize(Policy = "UserOrAdminPolicy")]
        public async Task<IActionResult> Create()
        {
            await MakeLists();
            var medicalCertificateViewModel = new CreateMedicalCertificateViewModel();

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
        [Authorize(Policy = "UserOrAdminPolicy")]
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
                    StudentID = (int)certificateViewModel.StudentID,
                    ClinicID = (int)certificateViewModel.ClinicID,
                    DiagnosisID = (int)certificateViewModel.DiagnosisID,
                    CertificatePath = certificateViewModel.CertificatePath,
                    IllnessDate = certificateViewModel.IlnessDate.HasValue ? certificateViewModel.IlnessDate.Value : default(DateTime),
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

        [Authorize(Policy = "UserOrAdminPolicy")]
        public async Task<IActionResult> Edit(int id)
        {
            var certificate = await _certificateRepository.GetIncludedByIdAsync(id);

            if (certificate == null)
            {
                return NotFound();
            }

            var certificateViewModel = new EditMedicalCertificateViewModel
            {
                CertificateID = certificate.CertificateID,
                StudentID = certificate.StudentID,
                Student = certificate.Student,
                ClinicID = certificate.ClinicID,
                DiagnosisID = certificate.DiagnosisID,
                CertificatePath = certificate.CertificatePath,
                IlnessDate = certificate.IllnessDate,
                RecoveryDate = certificate.RecoveryDate,
                Answer = certificate.Answer,
                CreatedAt = certificate.CreatedAt
            };

            await MakeLists();

            return View(certificateViewModel);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserOrAdminPolicy")]
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
                StudentID = (int)certificateViewModel.StudentID,
                ClinicID = (int)certificateViewModel.ClinicID,
                DiagnosisID = (int)certificateViewModel.DiagnosisID,
                CertificatePath = certificateViewModel.CertificatePath,
                IllnessDate = certificateViewModel.IlnessDate.HasValue ? certificateViewModel.IlnessDate.Value : default(DateTime),
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
            ModelState.Remove("StudentData");
            var certificates = await GetFilteredCertificates(filterViewModel);

            var paginatedCertificates = certificates
                .Skip((1 - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.StudentList = new SelectList(await GetStudentFullNamesWithGroups(), "Value", "Text");

            var newViewModel = new ShowMedicalCertificateViewModel
            {
                Certificates = paginatedCertificates,
                FilterViewModel = new FilterCertificatesViewModel
                {
                    StudentData = filterViewModel.StudentData,
                    RecoveryDate = filterViewModel.RecoveryDate,
                    IllnessDate = filterViewModel.IllnessDate,
                },
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = PageSize,
                    TotalItems = certificates.Count,
                    TotalPages = (int)Math.Ceiling((double)certificates.Count / PageSize)
                }
            };

            return View("Index", newViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Filter(int page = 1, string studentData = null, DateTime? illnessDate = null, DateTime? recoveryDate = null)
        {
            FilterCertificatesViewModel filterViewModel = new FilterCertificatesViewModel();

            filterViewModel.StudentData = studentData == null ? filterViewModel.StudentData : studentData;
            filterViewModel.IllnessDate = illnessDate == null ? filterViewModel.IllnessDate : illnessDate;
            filterViewModel.RecoveryDate = recoveryDate == null ? filterViewModel.RecoveryDate : recoveryDate;
            var certificates = await GetFilteredCertificates(filterViewModel);

            var paginatedCertificates = certificates
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.StudentList = new SelectList(await GetStudentFullNamesWithGroups(), "Value", "Text");

            var newViewModel = new ShowMedicalCertificateViewModel
            {
                Certificates = paginatedCertificates,
                FilterViewModel = filterViewModel,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = certificates.Count,
                    TotalPages = (int)Math.Ceiling((double)certificates.Count / PageSize)
                }
            };

            return View("Index", newViewModel);
        }

        private async Task<List<MedicalCertificate>> GetFilteredCertificates(FilterCertificatesViewModel filterViewModel)
        {
            List<MedicalCertificate> certificates = new List<MedicalCertificate>();

            if (filterViewModel.StudentData != null)
            {
                string[] studentData = filterViewModel.StudentData.Split(" - ");
                string[] fullName = studentData[0].Split();
                string groupName = studentData[1];
                string lastName = fullName[0];
                string firstName = fullName[1];
                string patronymic = fullName[2];
                Student foundStudent = await _studentRepository.GetByFullNameAndGroup(lastName, firstName, patronymic, groupName);
                certificates = await _certificateRepository.GetAllSortedAndIncludedByStudentIdAsync(foundStudent.StudentID);
            }

            var ilnessDate = filterViewModel.IllnessDate ?? default;
            var recoveryDate = filterViewModel.RecoveryDate ?? default;


            bool isValid = ilnessDate != DateTime.MinValue && recoveryDate != DateTime.MinValue;

            if (isValid && certificates.Count() != 0)
            {
                certificates = certificates.Select(c => c)
                    .Where(c => c.IllnessDate >= ilnessDate
                    && c.IllnessDate <= recoveryDate).ToList();
            }
            else if (isValid && certificates.Count == 0)
            {
                certificates = await _certificateRepository.GetAllByTimePeriod(ilnessDate, recoveryDate);
            }

            return certificates;
        }

        [Authorize(Policy = "UserOrAdminPolicy")]
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
        [Authorize(Policy = "UserOrAdminPolicy")]
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

        [HttpGet]
        public async Task<IActionResult> DownloadExcelReport()
        {
            var certificates = await _certificateRepository.GetAllIncludedAsync();
            var stream = GenerateExcel(certificates);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFilteredExcelReport(string studentData = null, DateTime? illnessDate = null, DateTime? recoveryDate = null)
        {
            var filteredCertificates = await GetFilteredCertificates(new FilterCertificatesViewModel
            {
                StudentData = studentData,
                IllnessDate = illnessDate,
                RecoveryDate = recoveryDate
            });
            filteredCertificates.Reverse();
            var stream = GenerateExcel(filteredCertificates);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "filtered_report.xlsx");
        }

        private MemoryStream GenerateExcel(IEnumerable<MedicalCertificate> certificates)
        {
            var stream = new MemoryStream();

            using (var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                var sheet = new Sheet { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Справки" };
                sheets.Append(sheet);

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Заголовки столбцов
                var headerRow = new Row();
                headerRow.Append(
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Номер заявки") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Фамилия") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Имя") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Отчество") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Группа") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Клиника") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Диагноз") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Болел с ") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Болел по") }
                // Добавьте остальные заголовки столбцов
                );
                sheetData.AppendChild(headerRow);

                // Данные из модели
                foreach (var certificate in certificates)
                {
                    var dataRow = new Row();
                    dataRow.Append(
                        new Cell() { DataType = CellValues.Number, CellValue = new CellValue(certificate.CertificateID.ToString()) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.LastName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.FirstName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.Patronymic) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.Group.GroupName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Clinic.ClinicName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Diagnosis.DiagnosisName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.IllnessDate.ToString("dd.MM.yyyy")) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.RecoveryDate.ToString("dd.MM.yyyy")) }
                    // Добавьте остальные данные справок
                    );
                    sheetData.AppendChild(dataRow);
                }
            }

            stream.Position = 0;
            return stream;
        }
    }
}
