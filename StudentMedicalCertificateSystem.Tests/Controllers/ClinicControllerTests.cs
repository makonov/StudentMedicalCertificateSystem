using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Controllers;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Controllers
{
    public class ClinicControllerTests
    {
        private ClinicController _clinicController;
        private IClinicRepository _clinicRepository;

        public ClinicControllerTests()
        {
            //Dependencies
            _clinicRepository = A.Fake<IClinicRepository>();

            //SUT
            _clinicController = new ClinicController(_clinicRepository);
        }

        [Fact]
        public async Task ClinicController_Index_ReturnsSuccess()
        {
            //Arrange
            var clinics = new List<Clinic>() { new Clinic(), new Clinic() };
            A.CallTo(() => _clinicRepository.GetAllSortedAsync()).Returns(clinics);

            //Act
            var result = await _clinicController.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<List<Clinic>>();
            viewResult.Model.Should().Be(clinics);
        }

        [Fact]
        public void ClinicController_Create_HttpGet_ReturnSuccess()
        {
            //Arrange

            //Act
            var result =  _clinicController.Create();

            //Assert
            result.Should().BeOfType<ViewResult>();

        }

        [Fact]
        public void ClinicController_Create_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var clinic = new Clinic { ClinicName = "Test"};

            // Act
            var result =  _clinicController.Create(clinic);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public void ClinicController_Create_HttpPost_InvalidModelState_ReturnsFailure()
        {
            //Arrange
            var clinic = new Clinic { ClinicName = null };
            _clinicController.ModelState.AddModelError("ClinicName", "");

            // Act
            var result = _clinicController.Create(clinic);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<Clinic>();
            _clinicController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ClinicController_Edit_HttpGet_ValidData_ReturnsSuccess()
        {
            //Arrange
            var id = 1;
            var clinic = new Clinic();
            A.CallTo(() => _clinicRepository.GetByIdAsync(id)).Returns(clinic);

            //Act
            var result = await _clinicController.Edit(id);

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task ClinicController_Edit_HttpGet_ClinicNotFound_ReturnsNotFound()
        {
            //Arrange
            var id = 1;
            Clinic clinic = null;
            A.CallTo(() => _clinicRepository.GetByIdAsync(id)).Returns(clinic);

            //Act
            var result = await _clinicController.Edit(id);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void ClinicController_Edit_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var clinic = new Clinic { ClinicName = "TEST"};

            // Act
            var result = _clinicController.Edit(1, clinic);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public void ClinicController_Edit_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var clinic = new Clinic { ClinicName = null };
            _clinicController.ModelState.AddModelError("ClinicName", "");

            // Act
            var result =  _clinicController.Edit(1, clinic);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<Clinic>();
            _clinicController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ClinicController_Delete_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var clinic = new Clinic { ClinicID = id, ClinicName = "TEST" };
            A.CallTo(() => _clinicRepository.GetByIdAsync(id)).Returns(Task.FromResult(clinic));

            // Act
            var result = await _clinicController.Delete(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task ClinicController_Delete_HttpGet_ClinicNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = 1;
            Clinic clinic = null;
            A.CallTo(() => _clinicRepository.GetByIdAsync(id)).Returns(Task.FromResult(clinic));

            // Act
            var result = await _clinicController.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ClinicController_Delete_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var clinic = new Clinic { ClinicID = id, ClinicName = "TEST" };
            A.CallTo(() => _clinicRepository.GetByIdAsync(id)).Returns(Task.FromResult(clinic));

            // Act
            var result = await _clinicController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task ClinicController_Delete_HttpPost_ClinicNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            Clinic clinic = null;
            A.CallTo(() => _clinicRepository.GetByIdAsync(id)).Returns(Task.FromResult(clinic));

            // Act
            var result = await _clinicController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ClinicController_Delete_ThrowsDbUpdateException()
        {
            // Arrange
            int id = 1;
            var clinic = new Clinic { ClinicID = id, ClinicName = "TEST" };
            A.CallTo(() => _clinicRepository.GetByIdAsync(clinic.ClinicID)).Returns(Task.FromResult(clinic));
            A.CallTo(() => _clinicRepository.Delete(clinic)).Throws<DbUpdateException>();
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            _clinicController.TempData = new TempDataDictionary(httpContext, tempDataProvider);
            // Act
            var result = await _clinicController.DeleteConfirmed(clinic.ClinicID);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
            _clinicController.TempData.ContainsKey("Error").Should().BeTrue();
            _clinicController.TempData["Error"].Should().Be("Данную больницу нельзя удалить, так как к ней прикреплены справки студентов");
        }
    }
}
