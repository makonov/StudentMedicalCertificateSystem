using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StudentMedicalCertificateSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.IntegrationTests
{
    public class MedicalCertificateControllerTest
    {
        [Test]
        public async Task Index_ReturnsViewWithViewModel()
        {
            // Arrange
            var certificateRepositoryMock = new Mock<IMedicalCertificateRepository>();
            var diagnosisRepositoryMock = new Mock<IDiagnosisRepository>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var clinicRepositoryMock = new Mock<IClinicRepository>();
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var programRepositoryMock = new Mock<IEducationalProgramRepository>();
            var photoServiceMock = new Mock<IPhotoService>();
            var userManagerMock = new Mock<UserManager<User>>();

            var controller = new MedicalCertificateController(
                certificateRepositoryMock.Object,
                diagnosisRepositoryMock.Object,
                userRepositoryMock.Object,
                clinicRepositoryMock.Object,
                studentRepositoryMock.Object,
                programRepositoryMock.Object,
                photoServiceMock.Object,
                userManagerMock.Object
            );

            var certificates = new List<MedicalCertificate>(); // Здесь создайте список сертификатов для тестирования
            certificateRepositoryMock.Setup(repo => repo.GetPagedCertificates(It.IsAny<int>(), It.IsAny<int>()))
                                     .ReturnsAsync(certificates);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ShowMedicalCertificateViewModel>(viewResult.Model);
            Assert.Equal(certificates, model.Certificates); // Проверяем, что возвращенная модель содержит список сертификатов
        }
    }
}
