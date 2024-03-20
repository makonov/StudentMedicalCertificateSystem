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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace StudentMedicalCertificateSystem.Controllers
{
    public class MedicalCertificateController : Controller
    {
        private readonly IMedicalCertificateRepository _certificateRepository;
        private readonly IDiagnosisRepository _diagnosisRepository;
        private readonly IUserRepository _userRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEducationalProgramRepository _programRepository;
        private readonly IPhotoService _photoService;
        private readonly UserManager<User> _userManager;

        private const int PageSize = 10;

        public MedicalCertificateController(IMedicalCertificateRepository certificateRepository,
            IDiagnosisRepository diagnosisRepository,
            IUserRepository userRepository,
            IClinicRepository clinicRepository,
            IStudentRepository studentRepository,
            IEducationalProgramRepository progragramRepository,
            IPhotoService photoService,
            UserManager<User> userManager)
        {
            _certificateRepository = certificateRepository;
            _diagnosisRepository = diagnosisRepository;
            _userRepository = userRepository;
            _clinicRepository = clinicRepository;
            _studentRepository = studentRepository;
            _programRepository = progragramRepository;
            _photoService = photoService;
            _userManager = userManager;
        }

        [Authorize(Policy = "AllRolesPolicy")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var totalCount = await _certificateRepository.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

            var certificates = await _certificateRepository.GetPagedCertificatesAsync(page, PageSize);
            ViewBag.StudentList = new SelectList(await GetStudentFullNamesWithGroups(), "Value", "Text");
            ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");

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

        private async Task<List<SelectListItem>> GetPrograms()
        {
            return await _programRepository.GetProgramsAsSelectList();
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
        public async Task<IActionResult> Create(CreateMedicalCertificateViewModel certificateViewModel)
        {
            if (!ModelState.IsValid)
            {
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

                var imageUploadResult = await _photoService.UploadPhotoAsync(certificateViewModel.Image, "Certificates");

                if (!imageUploadResult.IsUploadedAndExtensionValid)
                {
                    ModelState.AddModelError("Image", "Выберите файл в формате jpg, jpeg, png или pdf.");
                    await MakeLists();
                    return View(certificateViewModel);
                }

                var clinicAnswerUploadResult = await _photoService.UploadPhotoAsync(certificateViewModel.ClinicAnswer, "Confirmations");
                if (certificateViewModel.ClinicAnswer != null && !clinicAnswerUploadResult.IsUploadedAndExtensionValid)
                {
                    ModelState.AddModelError("ClinicAnswer", "Выберите файл в формате jpg, jpeg, png или pdf.");
                    await MakeLists();
                    return View(certificateViewModel);
                }

                var certificate = new MedicalCertificate
                {
                    StudentID = (int)certificateViewModel.StudentID,
                    ClinicID = (int)certificateViewModel.ClinicID,
                    DiagnosisID = (int)certificateViewModel.DiagnosisID,
                    UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CertificatePath = "/Certificates/" + imageUploadResult.FileName,
                    ClinicAnswerPath = clinicAnswerUploadResult.IsUploadedAndExtensionValid ? "/Confirmations/" + clinicAnswerUploadResult.FileName : null,
                    CertificateNumber = certificateViewModel.CertificateNumber,
                    IssueDate = certificateViewModel.IssueDate,
                    IllnessDate = certificateViewModel.IlnessDate,
                    RecoveryDate = certificateViewModel.RecoveryDate,
                    Answer = certificateViewModel.Answer,
                    IsConfirmed = certificateViewModel.IsConfirmed,
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

            string fullName = $"{certificate.Student.LastName} {certificate.Student.FirstName} {certificate.Student.Patronymic}";
            var certificateViewModel = new EditMedicalCertificateViewModel
            {
                CertificateID = certificate.CertificateID,
                StudentID = certificate.StudentID,
                FullName = fullName,
                ImagePath = certificate.CertificatePath,
                ClinicAnswerPath = certificate.ClinicAnswerPath,
                ClinicID = certificate.ClinicID,
                DiagnosisID = certificate.DiagnosisID,
                CertificateNumber = certificate.CertificateNumber,
                IssueDate = certificate.IssueDate,
                IlnessDate = certificate.IllnessDate,
                RecoveryDate = certificate.RecoveryDate,
                Answer = certificate.Answer,
                IsConfirmed = certificate.IsConfirmed,
                CreatedAt = certificate.CreatedAt
            };

            await MakeLists();

            return View(certificateViewModel);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserOrAdminPolicy")]
        public async Task<IActionResult> Edit(int id, EditMedicalCertificateViewModel certificateViewModel)
        {
            if (!ModelState.IsValid)
            {
                await MakeLists();
                return View(certificateViewModel);
            } 

            string imagePath = string.Empty;

            // Проверка на наличие и формат нового файла справки
            if (certificateViewModel.Image != null)
            {
                var replacementResult = await _photoService.ReplacePhotoAsync(certificateViewModel.Image, "Certificates", certificateViewModel.ImagePath);

                if (!replacementResult.IsReplacementSuccess)
                {
                    ModelState.AddModelError("Image", "Выберите файл в формате jpg, jpeg, png или pdf");
                    await MakeLists();
                    return View(certificateViewModel);
                }

                // Присвоение пути к новому файлу в модели
                imagePath = "/Certificates/" + replacementResult.NewFileName;
            }
            else
            {
                imagePath = certificateViewModel.ImagePath;
            }

            string clinicAnswerPath = string.Empty;

            // Проверка на наличие и формат файла ответа из больницы
            if (certificateViewModel.ClinicAnswer != null)
            {
                var updateResult = await _photoService.ReplacePhotoAsync(certificateViewModel.ClinicAnswer, "Confirmations", certificateViewModel.ClinicAnswerPath);

                if (!updateResult.IsReplacementSuccess)
                {
                    ModelState.AddModelError("Image", "Выберите файл в формате jpg, jpeg, png или pdf");
                    await MakeLists();
                    return View(certificateViewModel);
                }

                // Присвоение пути к новому файлу в модели
                clinicAnswerPath = "/Confirmations/" + updateResult.NewFileName;
            }
            else
            {
                clinicAnswerPath = certificateViewModel.ClinicAnswerPath;
            }

            var certificate = new MedicalCertificate
            {
                CertificateID = certificateViewModel.CertificateID,
                StudentID = (int)certificateViewModel.StudentID,
                ClinicID = (int)certificateViewModel.ClinicID,
                DiagnosisID = (int)certificateViewModel.DiagnosisID,
                CertificatePath = imagePath,
                ClinicAnswerPath= clinicAnswerPath,
                CertificateNumber = certificateViewModel.CertificateNumber,
                IssueDate = certificateViewModel.IssueDate,
                IllnessDate = certificateViewModel.IlnessDate,
                RecoveryDate = certificateViewModel.RecoveryDate,
                UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Answer = certificateViewModel.Answer,
                IsConfirmed = certificateViewModel.IsConfirmed,
                CreatedAt = certificateViewModel.CreatedAt,
                UpdatedAt = DateTime.Now
            };

            _certificateRepository.Update(certificate);
            return RedirectToAction("Index");
        }

        public IActionResult CreateStudentAndReturn()
        {
            return RedirectToAction("Create", "Student", new { isFromCertificate = true });
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
            ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");

            var newViewModel = new ShowMedicalCertificateViewModel
            {
                Certificates = paginatedCertificates,
                FilterViewModel = new FilterCertificatesViewModel
                {
                    StudentData = filterViewModel.StudentData,
                    ProgramID = filterViewModel.ProgramID,
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
        public async Task<IActionResult> Filter(int page = 1, string studentData = null, int? programId = null, DateTime? illnessDate = null, DateTime? recoveryDate = null)
        {
            FilterCertificatesViewModel filterViewModel = new FilterCertificatesViewModel
            {
                StudentData = studentData,
                ProgramID = programId,
                IllnessDate = illnessDate,
                RecoveryDate = recoveryDate
            };

            var certificates = await GetFilteredCertificates(filterViewModel);

            var paginatedCertificates = certificates
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.StudentList = new SelectList(await GetStudentFullNamesWithGroups(), "Value", "Text");
            ViewBag.ProgramList = new SelectList(await GetPrograms(), "Value", "Text");

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


            bool isDateValid = ilnessDate != DateTime.MinValue && recoveryDate != DateTime.MinValue;

            if (isDateValid && certificates.Count() != 0)
            {
                certificates = certificates.Select(c => c)
                    .Where(c => c.IllnessDate >= ilnessDate
                    && c.IllnessDate <= recoveryDate).ToList();
            }
            else if (isDateValid && certificates.Count == 0)
            {
                certificates = await _certificateRepository.GetAllByTimePeriodAsync(ilnessDate, recoveryDate);
            }

            if (filterViewModel.ProgramID != null && certificates.Count() != 0)
            {
                var filteredByProgram = await _certificateRepository.GetAllByProgramIdAsync((int) filterViewModel.ProgramID);
                certificates = filteredByProgram.Intersect(certificates).ToList();
            }
            else if (filterViewModel.ProgramID != null && certificates.Count() == 0)
            {
                certificates = await _certificateRepository.GetAllByProgramIdAsync((int)filterViewModel.ProgramID);
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

        [Authorize(Policy = "AllRolesPolicy")]
        public IActionResult ImageView(string imagePath)
        {
            return View("ImageView", imagePath);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateIsConfirmed(int certificateId, bool isChecked)
        {
            // Получите ваш сертификат по ID и обновите значение IsConfirmed
            var certificate = await  _certificateRepository.GetByIdAsync(certificateId);
            if (certificate != null)
            {
                certificate.IsConfirmed = isChecked;
                _certificateRepository.Update(certificate);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, error = "Certificate not found" });
            }
        }

        [HttpGet]
        [Authorize(Policy = "UserOrAdminPolicy")]
        public async Task<IActionResult> DownloadExcelReport()
        {
            var certificates = await _certificateRepository.GetAllIncludedAsync();
            var stream = GenerateExcel(certificates);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        }

        [HttpGet]
        [Authorize(Policy = "UserOrAdminPolicy")]
        public async Task<IActionResult> DownloadFilteredExcelReport(string studentData = null, int? programId = null, DateTime? illnessDate = null, DateTime? recoveryDate = null)
        {
            var filteredCertificates = await GetFilteredCertificates(new FilterCertificatesViewModel
            {
                ProgramID = programId,
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
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Фамилия") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Имя") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Отчество") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Образовательная программа") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Группа") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("№ справки") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Больница") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Диагноз") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Код диагноза") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Дата выдачи") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Болел с ") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Болел по") }
                );
                sheetData.AppendChild(headerRow);

                // Данные из модели
                foreach (var certificate in certificates)
                {
                    var dataRow = new Row();
                    dataRow.Append(
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.LastName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.FirstName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.Patronymic) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.Group.Program.ProgramName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Student.Group.GroupName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.CertificateNumber.HasValue ? certificate.CertificateNumber.ToString() : "") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Clinic.ClinicName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.Diagnosis.DiagnosisName) },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(!string.IsNullOrEmpty(certificate.Diagnosis.Code) ? certificate.Diagnosis.Code : "") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.IssueDate.HasValue ? certificate.IssueDate.Value.ToString("dd.MM.yyyy") : "") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.IllnessDate.HasValue ? certificate.IllnessDate.Value.ToString("dd.MM.yyyy") : "") },
                        new Cell() { DataType = CellValues.String, CellValue = new CellValue(certificate.RecoveryDate.HasValue ? certificate.RecoveryDate.Value.ToString("dd.MM.yyyy") : "") }

                    );
                    sheetData.AppendChild(dataRow);
                }
            }

            stream.Position = 0;
            return stream;
        }
    }
}
