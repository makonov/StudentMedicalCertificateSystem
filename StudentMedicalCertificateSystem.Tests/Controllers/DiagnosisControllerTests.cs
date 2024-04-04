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
    public class DiagnosisControllerTests
    {
        private DiagnosisController _diagnosisController;
        private IDiagnosisRepository _diagnosisRepository;

        public DiagnosisControllerTests()
        {
            //Dependencies
            _diagnosisRepository = A.Fake<IDiagnosisRepository>();

            //SUT
            _diagnosisController = new DiagnosisController(_diagnosisRepository);
        }

        [Fact]
        public async Task DiagnosisController_Index_ReturnsSuccess()
        {
            //Arrange
            var diagnoses = new List<Diagnosis>() { new Diagnosis(), new Diagnosis() };
            A.CallTo(() => _diagnosisRepository.GetAllSortedAsync()).Returns(diagnoses);

            //Act
            var result = await _diagnosisController.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<List<Diagnosis>>();
            viewResult.Model.Should().Be(diagnoses);
        }

        [Fact]
        public void DiagnosisController_Create_HttpGet_ReturnSuccess()
        {
            //Arrange

            //Act
            var result = _diagnosisController.Create();

            //Assert
            result.Should().BeOfType<ViewResult>();

        }

        [Fact]
        public void DiagnosisController_Create_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var diagnosis = new Diagnosis { DiagnosisName = "Test" };

            // Act
            var result = _diagnosisController.Create(diagnosis);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public void DiagnosisController_Create_HttpPost_InvalidModelState_ReturnsFailure()
        {
            //Arrange
            var diagnosis = new Diagnosis { DiagnosisName = null };
            _diagnosisController.ModelState.AddModelError("DiagnosisName", "");

            // Act
            var result = _diagnosisController.Create(diagnosis);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<Diagnosis>();
            _diagnosisController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task DiagnosisController_Edit_HttpGet_ValidData_ReturnsSuccess()
        {
            //Arrange
            var id = 1;
            var diagnosis = new Diagnosis();
            A.CallTo(() => _diagnosisRepository.GetByIdAsync(id)).Returns(diagnosis);

            //Act
            var result = await _diagnosisController.Edit(id);

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task DiagnosisController_Edit_HttpGet_DiagnosisNotFound_ReturnsNotFound()
        {
            //Arrange
            var id = 1;
            Diagnosis diagnosis = null;
            A.CallTo(() => _diagnosisRepository.GetByIdAsync(id)).Returns(diagnosis);

            //Act
            var result = await _diagnosisController.Edit(id);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void DiagnosisController_Edit_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var diagnosis = new Diagnosis { DiagnosisName = "TEST" };

            // Act
            var result = _diagnosisController.Edit(1, diagnosis);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public void DiagnosisController_Edit_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var diagnosis = new Diagnosis { DiagnosisName = null };
            _diagnosisController.ModelState.AddModelError("DiagnosisName", "");

            // Act
            var result = _diagnosisController.Edit(1, diagnosis);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<Diagnosis>();
            _diagnosisController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task DiagnosisController_Delete_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var diagnosis = new Diagnosis { DiagnosisID = id, DiagnosisName = "TEST" };
            A.CallTo(() => _diagnosisRepository.GetByIdAsync(id)).Returns(Task.FromResult(diagnosis));

            // Act
            var result = await _diagnosisController.Delete(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task DiagnosisController_Delete_HttpGet_DiagnosisNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = 1;
            Diagnosis diagnosis = null;
            A.CallTo(() => _diagnosisRepository.GetByIdAsync(id)).Returns(Task.FromResult(diagnosis));

            // Act
            var result = await _diagnosisController.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DiagnosisController_Delete_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var diagnosis = new Diagnosis { DiagnosisID = id, DiagnosisName = "TEST" };
            A.CallTo(() => _diagnosisRepository.GetByIdAsync(id)).Returns(Task.FromResult(diagnosis));

            // Act
            var result = await _diagnosisController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task DiagnosisController_Delete_HttpPost_DiagnosisNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            Diagnosis diagnosis = null;
            A.CallTo(() => _diagnosisRepository.GetByIdAsync(id)).Returns(Task.FromResult(diagnosis));

            // Act
            var result = await _diagnosisController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DiagnosisController_Delete_ThrowsDbUpdateException()
        {
            // Arrange
            int id = 1;
            var diagnosis = new Diagnosis { DiagnosisID = id, DiagnosisName = "TEST" };
            A.CallTo(() => _diagnosisRepository.GetByIdAsync(diagnosis.DiagnosisID)).Returns(Task.FromResult(diagnosis));
            A.CallTo(() => _diagnosisRepository.Delete(diagnosis)).Throws<DbUpdateException>();
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            _diagnosisController.TempData = new TempDataDictionary(httpContext, tempDataProvider);
            // Act
            var result = await _diagnosisController.DeleteConfirmed(diagnosis.DiagnosisID);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
            _diagnosisController.TempData.ContainsKey("Error").Should().BeTrue();
            _diagnosisController.TempData["Error"].Should().Be("Данный диагноз нельзя удалить, так как к нему прикреплены справки студентов");
        }
    }
}
