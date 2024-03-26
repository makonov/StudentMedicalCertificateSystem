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
            return await _programRepository.GetProgramsAsSelectListAsync();
        }

        private async Task<List<SelectListItem>> GetDiagnoses()
        {
            // Здесь получаем список диагнозов из базы данных
            return await _diagnosisRepository.GetDiagnosesAsSelectListAsync();
        }

        private async Task<List<SelectListItem>> GetClinics()
        {
            // Здесь получаем список работников из базы данных
            return await _clinicRepository.GetClinicsAsSelectListAsync();
        }

        private async Task<List<SelectListItem>> GetStudentFullNamesWithGroups()
        {
            // Здесь получаем список студентов из базы данных
            return await _studentRepository.GetFullNamesWithGroupsAsSelectedListAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserOrAdminPolicy")]
        public async Task<IActionResult> Create(CreateMedicalCertificateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await MakeLists();
                return View(viewModel);
            }

            // Поиск выбранного студента
            string[] fullName = viewModel.FullName.Split();
            string lastName = fullName[0];
            string firstName = fullName[1];
            string patronymic = fullName[2];
            Student foundStudent = await _studentRepository.GetDefaultByFullNameAsync(lastName, firstName, patronymic);

            // Если студент найден
            if (foundStudent != null)
            {
                viewModel.StudentID = foundStudent.StudentID;

                var imageUploadResult = await _photoService.UploadPhotoAsync(viewModel.Image, "Certificates");

                if (!imageUploadResult.IsUploadedAndExtensionValid)
                {
                    ModelState.AddModelError("Image", "Выберите файл в формате jpg, jpeg, png или pdf.");
                    await MakeLists();
                    return View(viewModel);
                }

                var clinicAnswerUploadResult = await _photoService.UploadPhotoAsync(viewModel.ClinicAnswer, "Confirmations");
                if (viewModel.ClinicAnswer != null && !clinicAnswerUploadResult.IsUploadedAndExtensionValid)
                {
                    ModelState.AddModelError("ClinicAnswer", "Выберите файл в формате jpg, jpeg, png или pdf.");
                    await MakeLists();
                    return View(viewModel);
                }

                var certificate = new MedicalCertificate
                {
                    StudentID = (int)viewModel.StudentID,
                    ClinicID = (int)viewModel.ClinicID,
                    DiagnosisID = (int)viewModel.DiagnosisID,
                    UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CertificatePath = "/Certificates/" + imageUploadResult.FileName,
                    ClinicAnswerPath = clinicAnswerUploadResult.IsUploadedAndExtensionValid ? "/Confirmations/" + clinicAnswerUploadResult.FileName : null,
                    CertificateNumber = viewModel.CertificateNumber,
                    IssueDate = viewModel.IssueDate,
                    IllnessDate = viewModel.IlnessDate,
                    RecoveryDate = viewModel.RecoveryDate,
                    Answer = viewModel.Answer,
                    IsConfirmed = viewModel.IsConfirmed,
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
                return View(viewModel);
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
        public async Task<IActionResult> Edit(int id, EditMedicalCertificateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await MakeLists();
                return View(viewModel);
            } 

            string imagePath = string.Empty;

            // Проверка на наличие и формат нового файла справки
            if (viewModel.Image != null)
            {
                var replacementResult = await _photoService.ReplacePhotoAsync(viewModel.Image, "Certificates", viewModel.ImagePath);

                if (!replacementResult.IsReplacementSuccess)
                {
                    ModelState.AddModelError("Image", "Выберите файл в формате jpg, jpeg, png или pdf");
                    await MakeLists();
                    return View(viewModel);
                }

                // Присвоение пути к новому файлу в модели
                imagePath = "/Certificates/" + replacementResult.NewFileName;
            }
            else
            {
                imagePath = viewModel.ImagePath;
            }

            string clinicAnswerPath = string.Empty;

            // Проверка на наличие и формат файла ответа из больницы
            if (viewModel.ClinicAnswer != null)
            {
                var updateResult = await _photoService.ReplacePhotoAsync(viewModel.ClinicAnswer, "Confirmations", viewModel.ClinicAnswerPath);

                if (!updateResult.IsReplacementSuccess)
                {
                    ModelState.AddModelError("Image", "Выберите файл в формате jpg, jpeg, png или pdf");
                    await MakeLists();
                    return View(viewModel);
                }

                // Присвоение пути к новому файлу в модели
                clinicAnswerPath = "/Confirmations/" + updateResult.NewFileName;
            }
            else
            {
                clinicAnswerPath = viewModel.ClinicAnswerPath;
            }

            var certificate = new MedicalCertificate
            {
                CertificateID = viewModel.CertificateID,
                StudentID = (int)viewModel.StudentID,
                ClinicID = (int)viewModel.ClinicID,
                DiagnosisID = (int)viewModel.DiagnosisID,
                CertificatePath = imagePath,
                ClinicAnswerPath= clinicAnswerPath,
                CertificateNumber = viewModel.CertificateNumber,
                IssueDate = viewModel.IssueDate,
                IllnessDate = viewModel.IlnessDate,
                RecoveryDate = viewModel.RecoveryDate,
                UserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Answer = viewModel.Answer,
                IsConfirmed = viewModel.IsConfirmed,
                CreatedAt = viewModel.CreatedAt,
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
        public async Task<IActionResult> Filter(FilterCertificatesViewModel viewModel)
        {
            ModelState.Remove("StudentData");
            var certificates = await GetFilteredCertificates(viewModel);

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
                    StudentData = viewModel.StudentData,
                    ProgramID = viewModel.ProgramID,
                    RecoveryDate = viewModel.RecoveryDate,
                    IllnessDate = viewModel.IllnessDate,
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

        private async Task<List<MedicalCertificate>> GetFilteredCertificates(FilterCertificatesViewModel viewModel)
        {
            List<MedicalCertificate> certificates = new List<MedicalCertificate>();

            if (viewModel.StudentData != null)
            {
                string[] studentData = viewModel.StudentData.Split(" - ");
                string[] fullName = studentData[0].Split();
                string groupName = studentData[1];
                string lastName = fullName[0];
                string firstName = fullName[1];
                string patronymic = fullName[2];
                Student foundStudent = await _studentRepository.GetByFullNameAndGroupAsync(lastName, firstName, patronymic, groupName);
                certificates = await _certificateRepository.GetAllSortedAndIncludedByStudentIdAsync(foundStudent.StudentID);
            }

            var ilnessDate = viewModel.IllnessDate ?? default;
            var recoveryDate = viewModel.RecoveryDate ?? default;


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

            if (viewModel.ProgramID != null && certificates.Count() != 0)
            {
                var filteredByProgram = await _certificateRepository.GetAllByProgramIdAsync((int)viewModel.ProgramID);
                certificates = filteredByProgram.Intersect(certificates).ToList();
            }
            else if (viewModel.ProgramID != null && certificates.Count() == 0)
            {
                certificates = await _certificateRepository.GetAllByProgramIdAsync((int)viewModel.ProgramID);
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
            // Получаем все сертификаты из репозитория
            var certificates = await _certificateRepository.GetAllIncludedAsync();
            // Генерируем отчет Excel
            var stream = GenerateExcel(certificates);
            // Возвращаем отчет в формате файла Excel
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        }

        [HttpGet]
        [Authorize(Policy = "UserOrAdminPolicy")]
        public async Task<IActionResult> DownloadFilteredExcelReport(string studentData = null, int? programId = null, DateTime? illnessDate = null, DateTime? recoveryDate = null)
        {
            // Получаем отфильтрованные сертификаты на основе переданных параметров
            var filteredCertificates = await GetFilteredCertificates(new FilterCertificatesViewModel
            {
                ProgramID = programId,
                StudentData = studentData,
                IllnessDate = illnessDate,
                RecoveryDate = recoveryDate
            });
            filteredCertificates.Reverse();
            // Генерируем отчет Excel на основе отфильтрованных сертификатов
            var stream = GenerateExcel(filteredCertificates);
            // Возвращаем отчет в формате файла Excel
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "filtered_report.xlsx");
        }

        private MemoryStream GenerateExcel(IEnumerable<MedicalCertificate> certificates)
        {
            // Создаем поток для записи данных отчета
            var stream = new MemoryStream();

            // Создаем документ Excel
            using (var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                // Добавляем часть книги Excel
                var workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                // Добавляем часть листа Excel
                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Создаем листы Excel
                var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                var sheet = new Sheet { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Справки" };
                sheets.Append(sheet);

                // Получаем данные листа Excel
                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Добавляем заголовки столбцов Excel
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

                // Добавляем данные сертификатов в Excel
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
            // Перемещаем указатель потока на начало
            stream.Position = 0;
            return stream;
        }
    }
}
