using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentMedicalCertificateSystem.Controllers;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;
using StudentMedicalCertificateSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Controllers
{
    public class EducationalProgramControllerTests
    {
        private EducationalProgramController _programController;
        private IEducationalProgramRepository _programRepository;

        public EducationalProgramControllerTests()
        {
            //Dependencies
            _programRepository = A.Fake<IEducationalProgramRepository>();

            //SUT
            _programController = new EducationalProgramController(_programRepository);
        }

        [Fact]
        public async Task EducationalProgramController_Index_ReturnsSuccess()
        {
            //Arrange
            var programs = new List<EducationalProgram>() { new EducationalProgram()};
            A.CallTo(() => _programRepository.GetAllSortedAsync()).Returns(programs);

            //Act
            var result = await _programController.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<List<EducationalProgram>>();
            viewResult.Model.Should().Be(programs);
        }

        [Fact]
        public void EducationalProgramController_Create_HttpGet_ReturnSuccess()
        {
            //Arrange

            //Act
            var result = _programController.Create();

            //Assert
            result.Should().BeOfType<ViewResult>();

        }

        [Fact]
        public void EducationalProgramController_Create_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var program = new EducationalProgram { ProgramName = "Test" };

            // Act
            var result = _programController.Create(program);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public void EducationalProgramController_Create_HttpPost_ProgramNotFound_ReturnsFailure()
        {
            //Arrange
            var program = new EducationalProgram { ProgramName = null };
            _programController.ModelState.AddModelError("ProgramName", "");

            // Act
            var result = _programController.Create(program);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<EducationalProgram>();
            _programController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void EducationalProgramControllerController_Edit_HttpGet_ValidData_ReturnsSuccess()
        {
            //Arrange
            var id = 1;
            var program = new EducationalProgram { ProgramName = "program"};
            A.CallTo(() => _programRepository.GetByIdAsync(id)).Returns(program);

            //Act
            var result = _programController.Edit(id);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }

        [Fact]
        public async Task EducationalProgramController_Edit_HttpGet_ProgramNotFound_ReturnsNotFound()
        {
            //Arrange
            var id = 1;
            EducationalProgram program = null ;
            A.CallTo(() => _programRepository.GetByIdAsync(id)).Returns(program);

            //Act
            var result = await _programController.Edit(id);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void EducationalProgramController_Edit_HttpPost_ValidData_ReturnSuccess()
        {
            // Arrange
            var program = new EducationalProgram {ProgramName = "Test" };

            // Act
            var result = _programController.Edit(1, program);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public void EducationalProgramController_Edit_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var program = new EducationalProgram { ProgramName = null };
            _programController.ModelState.AddModelError("ProgramName", "");

            // Act
            var result = _programController.Edit(1, program);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<EducationalProgram>();
            _programController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task EducationalProgramController_Delete_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var program = new EducationalProgram { ProgramName = "program"};
            A.CallTo(() => _programRepository.GetByIdAsync(id)).Returns(Task.FromResult(program));

            // Act
            var result = await _programController.Delete(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task EducationalProgramController_Delete_HttpGet_ProgramNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            EducationalProgram program = null;
            A.CallTo(() => _programRepository.GetByIdAsync(id)).Returns(Task.FromResult(program));

            // Act
            var result = await _programController.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EducationalProgramController_Delete_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var program = new EducationalProgram { ProgramName = "program"};
            A.CallTo(() => _programRepository.GetByIdAsync(id)).Returns(Task.FromResult(program));

            // Act
            var result = await _programController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task EducationalProgramController_Delete_HttpPost_ProgramNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            EducationalProgram program = null;
            A.CallTo(() => _programRepository.GetByIdAsync(id)).Returns(Task.FromResult(program));

            // Act
            var result = await _programController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EducationalProgramController_Delete_ThrowsDbUpdateException()
        {
            // Arrange
            int id = 1;
            var program = new EducationalProgram { ProgramID = id, ProgramName = "TEST" };
            A.CallTo(() => _programRepository.GetByIdAsync(program.ProgramID)).Returns(Task.FromResult(program));
            A.CallTo(() => _programRepository.Delete(program)).Throws<DbUpdateException>();
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            _programController.TempData = new TempDataDictionary(httpContext, tempDataProvider);
            // Act
            var result = await _programController.DeleteConfirmed(program.ProgramID);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
            _programController.TempData.ContainsKey("Error").Should().BeTrue();
            _programController.TempData["Error"].Should().Be("Данную образовательную программу нельзя удалить, так как к ней привязаны группы");
        }
    }
}
