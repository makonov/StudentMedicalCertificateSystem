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
using System.Linq;
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
        public async Task MedicalCertificateController_Index_ReturnSuccess()
        {
            //Arrange
            //var certificates = A.Fake<List<MedicalCertificate>>();
            var expectedCertificates = new List<MedicalCertificate>
            {
                new MedicalCertificate { /* Заполните поля сертификата */ },
                new MedicalCertificate { /* Заполните поля сертификата */ },
                // Добавьте любое необходимое количество сертификатов
            };
            var expectedViewModel = new ShowMedicalCertificateViewModel() { Certificates = expectedCertificates };
            A.CallTo(() => _certificateRepository.CountAsync()).Returns(2);
            A.CallTo(() => _certificateRepository.GetPagedCertificatesAsync(A<int>._, A<int>._)).Returns(Task.FromResult(expectedCertificates));

            //Act
            var result = _certificateController.Index();

            // Assert
            var viewResult = await Assert.IsType<Task<IActionResult>>(result) as ViewResult; // Подождать выполнения задачи и убедиться, что она возвращает IActionResult
            var viewModel = Assert.IsType<ShowMedicalCertificateViewModel>(viewResult.Model); // Проверить, что модель представления соответствует ожидаемому типу
            Assert.Same(expectedViewModel.Certificates, viewModel.Certificates);
        }

        [Fact]
        public async Task MedicalCertificateController_Edit_HttpGet_ReturnSuccess()
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
    }
}
