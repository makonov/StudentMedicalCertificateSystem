using DocumentFormat.OpenXml.Wordprocessing;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Controllers;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Controllers
{
    public class MedicalCertificateControllerTests
    {
        private MedicalCertificateController _certificateController;

        private IMedicalCertificateRepository _certificateRepository;
        private IDiagnosisRepository _diagnosisRepository;
        private IUserRepository _userRepository;
        private IClinicRepository _clinicRepository;
        private IStudentRepository _studentRepository;
        private IEducationalProgramRepository _programRepository;
        private IPhotoService _photoService;
        private UserManager<User> _userManager;
        private IHttpContextAccessor _httpContextAccessor;

        private const int PageSize = 10;

        public MedicalCertificateControllerTests()
        {
            //Dependencies
            _certificateRepository = A.Fake<IMedicalCertificateRepository>();
            _diagnosisRepository = A.Fake<IDiagnosisRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _clinicRepository = A.Fake<IClinicRepository>();
            _studentRepository = A.Fake<IStudentRepository>();
            _programRepository = A.Fake<IEducationalProgramRepository>();
            _photoService = A.Fake<IPhotoService>();
            _userManager = A.Fake<UserManager<User>>();
            _httpContextAccessor = A.Fake<HttpContextAccessor>();

            //SUT
            _certificateController = new MedicalCertificateController(_certificateRepository,
                _diagnosisRepository,
                _userRepository,
                _clinicRepository,
                _studentRepository,
                _programRepository,
                _photoService,
                _userManager);
        }

        [Fact]
        public async Task MedicalCertificateController_Index_ReturnsSuccess()
        {
            //Arrange
            var certificates = new List<MedicalCertificate>
            {
                new MedicalCertificate(),
                new MedicalCertificate(),
                new MedicalCertificate(),
                new MedicalCertificate()
            };
            var expectedViewModel = new ShowMedicalCertificateViewModel() { Certificates = certificates };
            A.CallTo(() => _certificateRepository.CountAsync()).Returns(2);
            A.CallTo(() => _certificateRepository.GetPagedCertificatesAsync(A<int>._, A<int>._)).Returns(Task.FromResult(certificates));

            //Act
            var result = await _certificateController.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<ShowMedicalCertificateViewModel>();
        }

        [Fact]
        public async Task MedicalCertificateController_Create_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var viewModel = new CreateMedicalCertificateViewModel
            {
                FullName = "LastName FirstName Patrobynic",
                StudentID = 5,
                ClinicID = 3,
                DiagnosisID = 10,
                Image = A.Fake<IFormFile>(),
                ClinicAnswer = A.Fake<IFormFile>(),
                CertificateNumber = 55,
                IssueDate = DateTime.Now,
                IlnessDate = DateTime.Now,
                RecoveryDate = DateTime.Now,
                Answer = "",
                IsConfirmed = true
            };

            Student foundStudent = A.Fake<Student>();

            var imageUploadResult = (true, "fileName");
            A.CallTo(() => _studentRepository.GetDefaultByFullNameAsync(A<string>._, A<string>._, A<string>._)).Returns(Task.FromResult(foundStudent));
            A.CallTo(() => _photoService.UploadPhotoAsync(A<IFormFile>._, A<string>._)).Returns(Task.FromResult(imageUploadResult));

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "userId")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var httpContext = new DefaultHttpContext
            {
                User = userPrincipal
            };
            _certificateController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _certificateController.Create(viewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task MedicalCertificateController_Create_HttpPost_InvalidExtension_ReturnsFailure()
        {
            // Arrange
            var viewModel = new CreateMedicalCertificateViewModel
            {
                FullName = "LastName FirstName Patrobynic",
                StudentID = 5,
                ClinicID = 3,
                DiagnosisID = 10,
                Image = A.Fake<IFormFile>(),
                ClinicAnswer = A.Fake<IFormFile>(),
                CertificateNumber = 55,
                IssueDate = DateTime.Now,
                IlnessDate = DateTime.Now,
                RecoveryDate = DateTime.Now,
                Answer = "",
                IsConfirmed = true
            };

            Student foundStudent = A.Fake<Student>();

            var imageUploadResult = (false, "fileName");
            A.CallTo(() => _studentRepository.GetDefaultByFullNameAsync(A<string>._, A<string>._, A<string>._)).Returns(Task.FromResult(foundStudent));
            A.CallTo(() => _photoService.UploadPhotoAsync(A<IFormFile>._, A<string>._)).Returns(Task.FromResult(imageUploadResult));

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "userId")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var httpContext = new DefaultHttpContext
            {
                User = userPrincipal
            };
            _certificateController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _certificateController.Create(viewModel);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNull();
            viewResult.Model.Should().BeOfType<CreateMedicalCertificateViewModel>();
            _certificateController.ModelState.Should().ContainKey("Image");
            _certificateController.ModelState["Image"].Errors.Should().Contain(error => error.ErrorMessage == "Выберите файл в формате jpg, jpeg, png или pdf.");
        }

        [Fact]
        public async Task MedicalCertificateController_Create_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var viewModel = new CreateMedicalCertificateViewModel
            {
                FullName = "LastName FirstName Patrobynic",
                StudentID = 5,
                ClinicID = null,
                DiagnosisID = 10,
                Image = A.Fake<IFormFile>(),
                ClinicAnswer = A.Fake<IFormFile>(),
                CertificateNumber = 55,
                IssueDate = DateTime.Now,
                IlnessDate = DateTime.Now,
                RecoveryDate = DateTime.Now,
                Answer = "",
                IsConfirmed = true
            };

            _certificateController.ModelState.AddModelError("ClinicID", "");

            // Act
            var result = await _certificateController.Create(viewModel);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNull();
            viewResult.Model.Should().BeOfType<CreateMedicalCertificateViewModel>();
            _certificateController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void MedicalCertificateController_Create_HttpGet_ReturnSuccess()
        {
            //Arrange

            //Act
            var result = _certificateController.Create();

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();

        }

        [Fact]
        public void MedicalCertificateController_Edit_HttpGet_ReturnSuccess()
        {
            //Arrange
            var id = 1;
            var certificate = A.Fake<MedicalCertificate>();
            A.CallTo(() => _certificateRepository.GetIncludedByIdAsync(id)).Returns(certificate);

            string fullName = $"{certificate.Student.LastName} {certificate.Student.FirstName} {certificate.Student.Patronymic}";
            var viewModel = new EditMedicalCertificateViewModel
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

            //Act
            var result = _certificateController.Edit(id);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();

        }

        [Fact]
        public async Task MedicalCertificateController_Edit_HttpPost_ValidData_ReturnSuccess()
        {
            // Arrange
            var viewModel = new EditMedicalCertificateViewModel
            {
                CertificateID = 121,
                FullName = "LastName FirstName Patrobynic",
                StudentID = 5,
                ClinicID = 3,
                DiagnosisID = 10,
                ImagePath = "imagePath",
                ClinicAnswerPath = "clinicAnswerPath",
                Image = A.Fake<IFormFile>(),
                ClinicAnswer = A.Fake<IFormFile>(),
                CertificateNumber = 55,
                IssueDate = DateTime.Now,
                IlnessDate = DateTime.Now,
                RecoveryDate = DateTime.Now,
                Answer = "",
                IsConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var replacementResult = (true, "newFileName");
            A.CallTo(() => _photoService.ReplacePhotoAsync(A<IFormFile>._, A<string>._, A<string>._)).Returns(Task.FromResult(replacementResult));

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "userId")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var httpContext = new DefaultHttpContext
            {
                User = userPrincipal
            };
            _certificateController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _certificateController.Edit(viewModel.CertificateID, viewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task MedicalCertificateController_Edit_HttpPost_ImageNotUpdated_ReturnSuccess()
        {
            // Arrange
            var viewModel = new EditMedicalCertificateViewModel
            {
                CertificateID = 121,
                FullName = "LastName FirstName Patrobynic",
                StudentID = 5,
                ClinicID = 3,
                DiagnosisID = 10,
                ImagePath = "imagePath",
                ClinicAnswerPath = "clinicAnswerPath",
                CertificateNumber = 55,
                IssueDate = DateTime.Now,
                IlnessDate = DateTime.Now,
                RecoveryDate = DateTime.Now,
                Answer = "",
                IsConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "userId")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var httpContext = new DefaultHttpContext
            {
                User = userPrincipal
            };
            _certificateController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _certificateController.Edit(viewModel.CertificateID, viewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task MedicalCertificateController_Edit_HttpPost_InvalidExtension_ReturnsFailure()
        {
            // Arrange
            var viewModel = new EditMedicalCertificateViewModel
            {
                CertificateID = 121,
                FullName = "LastName FirstName Patrobynic",
                StudentID = 5,
                ClinicID = 3,
                DiagnosisID = 10,
                ImagePath = "imagePath",
                ClinicAnswerPath = "clinicAnswerPath",
                Image = A.Fake<IFormFile>(),
                ClinicAnswer = A.Fake<IFormFile>(),
                CertificateNumber = 55,
                IssueDate = DateTime.Now,
                IlnessDate = DateTime.Now,
                RecoveryDate = DateTime.Now,
                Answer = "",
                IsConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var replacementResult = (false, "newFileName");
            A.CallTo(() => _photoService.ReplacePhotoAsync(A<IFormFile>._, A<string>._, A<string>._)).Returns(Task.FromResult(replacementResult));

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "userId")
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuth");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var httpContext = new DefaultHttpContext
            {
                User = userPrincipal
            };
            _certificateController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _certificateController.Edit(viewModel.CertificateID, viewModel);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNull();
            viewResult.Model.Should().BeOfType<EditMedicalCertificateViewModel>();
            _certificateController.ModelState.Should().ContainKey("Image");
            _certificateController.ModelState["Image"].Errors.Should().Contain(error => error.ErrorMessage == "Выберите файл в формате jpg, jpeg, png или pdf.");
        }

        [Fact]
        public async Task MedicalCertificateController_Edit_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var viewModel = new EditMedicalCertificateViewModel
            {
                CertificateID = 121,
                FullName = "LastName FirstName Patrobynic",
                StudentID = 5,
                ClinicID = null,
                DiagnosisID = 10,
                ImagePath = "imagePath",
                ClinicAnswerPath = "clinicAnswerPath",
                Image = A.Fake<IFormFile>(),
                ClinicAnswer = A.Fake<IFormFile>(),
                CertificateNumber = 55,
                IssueDate = DateTime.Now,
                IlnessDate = DateTime.Now,
                RecoveryDate = DateTime.Now,
                Answer = "",
                IsConfirmed = true,
                CreatedAt = DateTime.Now
            };
            _certificateController.ModelState.AddModelError("ClinicID", "");

            // Act
            var result = await _certificateController.Edit(viewModel.CertificateID, viewModel); ;

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewName.Should().BeNull();
            viewResult.Model.Should().BeOfType<EditMedicalCertificateViewModel>();
            _certificateController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task MedicalCertificateController_Edit_HttpGet_CertificateNotFound_ReturnsNotFound()
        {
            //Arrange
            var id = 1;
            MedicalCertificate certificate = null;
            A.CallTo(() => _certificateRepository.GetIncludedByIdAsync(id)).Returns(Task.FromResult(certificate));

            //Act
            var result = await _certificateController.Edit(id);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        private List<MedicalCertificate> GetListOfMedicalCertificates()
        {
            var certificate1 = new MedicalCertificate()
            {
                Student = new Student()
                {
                    Group = new StudentGroup()
                    {
                        Program = new EducationalProgram()
                        {
                            ProgramName = "Программная инженерия"
                        },
                        GroupName = "ПИ-21-1"
                    },
                    LastName = "Иванов",
                    FirstName = "Иван",
                    Patronymic = "Иванович",
                    BirthDate = DateTime.ParseExact("16.02.2002", "dd.MM.yyyy", CultureInfo.InvariantCulture)
                },
                User = new User()
                {
                    UserName = "officeWorker",
                    PasswordHash = "HashCode",
                    LastName = "Сахипова",
                    FirstName = "Марина",
                    Patronymic = "Станиславовна"
                },
                Clinic = new Clinic()
                {
                    ClinicName = "Больница №1"
                },
                Diagnosis = new Diagnosis()
                {
                    DiagnosisName = "ОРВИ",
                    Code = "J9.06"
                },
                CertificateNumber = 55,
                CertificatePath = "certificatePath",
                ClinicAnswerPath = "clinicAnswerPath",
                IsConfirmed = true,
                IssueDate = DateTime.ParseExact("16.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                IllnessDate = DateTime.ParseExact("02.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("15.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Answer = "нет",
                CreatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                UpdatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            var certificate2 = new MedicalCertificate()
            {
                Student = certificate1.Student,
                User = new User()
                {
                    UserName = "officeWorker",
                    PasswordHash = "HashCode",
                    LastName = "Сахипова",
                    FirstName = "Марина",
                    Patronymic = "Станиславовна"
                },
                Clinic = new Clinic()
                {
                    ClinicName = "Больница №1"
                },
                Diagnosis = new Diagnosis()
                {
                    DiagnosisName = "ОРВИ",
                    Code = "J9.06"
                },
                CertificateNumber = 55,
                CertificatePath = "certificatePath",
                ClinicAnswerPath = "clinicAnswerPath",
                IsConfirmed = true,
                IssueDate = DateTime.ParseExact("16.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                IllnessDate = DateTime.ParseExact("02.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("15.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Answer = "нет",
                CreatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                UpdatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            var certificate3 = new MedicalCertificate()
            {
                Student = new Student()
                {
                    Group = new StudentGroup()
                    {
                        Program = new EducationalProgram()
                        {
                            ProgramName = "Бизнес-информатика"
                        },
                        GroupName = "БИ-21-1"
                    },
                    LastName = "Петров",
                    FirstName = "Петр",
                    Patronymic = "Петрович",
                    BirthDate = DateTime.ParseExact("16.02.2002", "dd.MM.yyyy", CultureInfo.InvariantCulture)
                },
                User = new User()
                {
                    UserName = "officeWorker",
                    PasswordHash = "HashCode",
                    LastName = "Сахипова",
                    FirstName = "Марина",
                    Patronymic = "Станиславовна"
                },
                Clinic = new Clinic()
                {
                    ClinicName = "Больница №1"
                },
                Diagnosis = new Diagnosis()
                {
                    DiagnosisName = "ОРВИ",
                    Code = "J9.06"
                },
                CertificateNumber = 55,
                CertificatePath = "certificatePath",
                ClinicAnswerPath = "clinicAnswerPath",
                IsConfirmed = true,
                IssueDate = DateTime.ParseExact("16.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                IllnessDate = DateTime.ParseExact("02.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("15.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Answer = "нет",
                CreatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                UpdatedAt = DateTime.ParseExact("17.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            var certificate4 = new MedicalCertificate()
            {
                Student = certificate1.Student,
                User = new User()
                {
                    UserName = "officeWorker",
                    PasswordHash = "HashCode",
                    LastName = "Сахипова",
                    FirstName = "Марина",
                    Patronymic = "Станиславовна"
                },
                Clinic = new Clinic()
                {
                    ClinicName = "Больница №1"
                },
                Diagnosis = new Diagnosis()
                {
                    DiagnosisName = "ОРВИ",
                    Code = "J9.06"
                },
                CertificateNumber = 55,
                CertificatePath = "certificatePath",
                ClinicAnswerPath = "clinicAnswerPath",
                IsConfirmed = true,
                IssueDate = DateTime.ParseExact("16.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                IllnessDate = DateTime.ParseExact("02.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("15.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Answer = "нет",
                CreatedAt = DateTime.ParseExact("17.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                UpdatedAt = DateTime.ParseExact("17.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };
            return new List<MedicalCertificate> { certificate1, certificate2, certificate3, certificate4 };
        }

        [Fact]
        public async Task MedicalCertificateController_Filter_HttpPost_ReturnsSuccess()
        {
            // Arrange
            var filterViewModel = new FilterCertificatesViewModel
            {
                StudentData = "Иванов Иван Иванович - ПИ-21-1",
                ProgramID = 1,
                IllnessDate = DateTime.ParseExact("01.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("19.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };

            var certificates = GetListOfMedicalCertificates();
            var certificatesById = new List<MedicalCertificate>() { certificates[0], certificates[1] };
            var student = certificates[0].Student;

            A.CallTo(() => _studentRepository.GetByFullNameAndGroupAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .Returns(Task.FromResult(student));
            A.CallTo(() => _certificateRepository.GetAllSortedAndIncludedByStudentIdAsync(A<int>._))
                .Returns(Task.FromResult(certificatesById));

            // Act
            var result = await _certificateController.Filter(filterViewModel) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Index");
            result.Model.Should().BeOfType<ShowMedicalCertificateViewModel>();
        }

        [Fact]
        public async Task MedicalCertificateController_Filter_HttpGet_ReturnsSuccess()
        {
            // Arrange
            var page = 1;
            var viewModel = new FilterCertificatesViewModel
            {
                ProgramID = 1,
                IllnessDate = DateTime.ParseExact("01.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("19.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };

            var certificates = GetListOfMedicalCertificates();
            var filteredCertificates = new List<MedicalCertificate>() { certificates[0], certificates[1], certificates[3] };
            A.CallTo(() => _certificateRepository.GetAllByTimePeriodAsync(A<DateTime>._, A<DateTime>._))
                .Returns(Task.FromResult(certificates));
            A.CallTo(() => _certificateRepository.GetAllByProgramIdAsync(A<int>._))
                .Returns(Task.FromResult(filteredCertificates));

            // Act
            var result = await _certificateController.Filter(page, viewModel.StudentData, viewModel.ProgramID, viewModel.IllnessDate, viewModel.RecoveryDate);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("Index");
        }

        [Fact]
        public async Task MedicalCertificateController_Delete_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var certificate = GetListOfMedicalCertificates()[0];
            A.CallTo(() => _certificateRepository.GetIncludedByIdAsync(id)).Returns(Task.FromResult(certificate));

            // Act
            var result = await _certificateController.Delete(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task MedicalCertificateController_Delete_HttpGet_CertificateNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            MedicalCertificate certificate = null;
            A.CallTo(() => _certificateRepository.GetIncludedByIdAsync(id)).Returns(Task.FromResult(certificate));

            // Act
            var result = await _certificateController.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task MedicalCertificateController_Delete_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var certificate = GetListOfMedicalCertificates()[0];
            A.CallTo(() => _certificateRepository.GetByIdAsync(id)).Returns(Task.FromResult(certificate));

            // Act
            var result = await _certificateController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task MedicalCertificateController_Delete_HttpPost_CertificateNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5; 
            MedicalCertificate certificate = null;
            A.CallTo(() => _certificateRepository.GetByIdAsync(id)).Returns(Task.FromResult(certificate));

            // Act
            var result = await _certificateController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void MedicalCertificateController_ImageView_ReturnsSuccess()
        {
            // Arrange
            var imagePath = "/path/to/image.jpg";

            // Act
            var result = _certificateController.ImageView(imagePath);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.ViewName.Should().Be("ImageView");
            viewResult.Model.Should().Be(imagePath);
        }

        [Fact]
        public async Task MedicalCertificateController_UpdateIsConfirmed_ValidData_ReturnsSuccess()
        {
            // Arrange
            var certificateId = 1;
            var isChecked = true;
            var certificate = new MedicalCertificate { CertificateID = certificateId, IsConfirmed = false };
            A.CallTo(() => _certificateRepository.GetByIdAsync(certificateId)).Returns(Task.FromResult(certificate));

            // Act
            var result = await _certificateController.UpdateIsConfirmed(certificateId, isChecked);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            certificate.IsConfirmed.Should().Be(isChecked);
            A.CallTo(() => _certificateRepository.Update(certificate)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task MedicalCertificateController_UpdateIsConfirmed_InvalidData_ReturnsFailure()
        {
            //Arrange
            var certificateId = 1;
            var isChecked = true;
            A.CallTo(() => _certificateRepository.GetByIdAsync(certificateId)).Returns(Task.FromResult<MedicalCertificate>(null));

            // Act
            var result = await _certificateController.UpdateIsConfirmed(certificateId, isChecked);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            A.CallTo(() => _certificateRepository.Update(A<MedicalCertificate>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task MedicalCertificateController_DownloadExcelReport_ReturnsFileResult()
        {
            // Arrange
            var certificates = GetListOfMedicalCertificates();
            A.CallTo(() => _certificateRepository.GetAllIncludedAsync()).Returns(certificates);

            // Act
            var result = await _certificateController.DownloadExcelReport();

            // Assert
            result.Should().BeOfType<FileContentResult>();
            var fileResult = result as FileContentResult;
            fileResult.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fileResult.FileDownloadName.Should().Be("report.xlsx");
            fileResult.FileContents.Should().NotBeNull();
        }

        [Fact]
        public async Task MedicalCertificateController_DownloadFilteredExcelReport_ReturnsFileResult()
        {
            // Arrange
            var filter = new FilterCertificatesViewModel
            {
                StudentData = "Иванов Иван Иванович - ПИ-21-1",
                ProgramID = 1,
                IllnessDate = DateTime.ParseExact("01.01.2024", "dd.MM.yyyy", CultureInfo.InvariantCulture),
                RecoveryDate = DateTime.ParseExact("19.01.2023", "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };

            // Act
            var result = await _certificateController.DownloadFilteredExcelReport(filter.StudentData, filter.ProgramID, filter.IllnessDate, filter.RecoveryDate);

            // Assert
            result.Should().BeOfType<FileContentResult>();
            var fileResult = result as FileContentResult;
            fileResult.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fileResult.FileDownloadName.Should().Be("filtered_report.xlsx");
            fileResult.FileContents.Should().NotBeNull();
        }

    }
}
