using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Controllers;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;
using StudentMedicalCertificateSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Controllers
{
    public class StudentGroupControllerTests
    {
        private StudentGroupController _groupController;
        private IEducationalProgramRepository _programRepository;
        private IStudentGroupRepository _groupRepository;

        public StudentGroupControllerTests()
        {
            //Dependencies
            _programRepository = A.Fake<IEducationalProgramRepository>();
            _groupRepository = A.Fake<IStudentGroupRepository>();

            //SUT
            _groupController = new StudentGroupController(_groupRepository, _programRepository);
        }

        [Fact]
        public async Task StudentGroupController_Index_ReturnsSuccess()
        {
            //Arrange
            var groups = new List<StudentGroup>() { new StudentGroup() };
            A.CallTo(() => _groupRepository.GetAllSortedAsync()).Returns(groups);

            //Act
            var result = await _groupController.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<List<StudentGroup>>();
            viewResult.Model.Should().Be(groups);
        }

        [Fact]
        public async Task StudentGroupController_Create_HttpGet_ReturnSuccess()
        {
            //Arrange

            //Act
            var result = await _groupController.Create();

            //Assert
            result.Should().BeOfType<ViewResult>();

        }

        [Fact]
        public async Task StudentGroupController_Create_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var group = new StudentGroup { GroupName = "Test", ProgramID = 1 };

            // Act
            var result = await _groupController.Create(group);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task StudentGroupController_Create_HttpPost_GroupNotFound_ReturnsFailure()
        {
            //Arrange
            var group = new StudentGroup { GroupName = null };
            _groupController.ModelState.AddModelError("ProgramName", "");

            // Act
            var result = await _groupController.Create(group);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<StudentGroup>();
            _groupController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task StudentGroupController_Edit_HttpGet_ValidData_ReturnsSuccess()
        {
            //Arrange
            var id = 1;
            var group = new StudentGroup();
            A.CallTo(() => _groupRepository.GetByIdAsync(id)).Returns(group);

            //Act
            var result = await _groupController.Edit(id);

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task StudentGroupController_Edit_HttpGet_GroupNotFound_ReturnsNotFound()
        {
            //Arrange
            var id = 1;
            StudentGroup group = null;
            A.CallTo(() => _groupRepository.GetByIdAsync(id)).Returns(group);

            //Act
            var result = await _groupController.Edit(id);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task StudentGroupController_Edit_HttpPost_ValidData_ReturnSuccess()
        {
            // Arrange
            var group = new StudentGroup { GroupName = "Test" , ProgramID = 5};

            // Act
            var result = await _groupController.Edit(1, group);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task StudentGroupController_Edit_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var group = new StudentGroup { GroupName = null, ProgramID = 5 };
            _groupController.ModelState.AddModelError("GroupName", "");

            // Act
            var result = await _groupController.Edit(1, group);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<StudentGroup>();
            _groupController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task StudentGroupController_Delete_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var group = new StudentGroup { GroupName = "group", ProgramID = 5};
            A.CallTo(() => _groupRepository.GetByIdAsync(id)).Returns(Task.FromResult(group));

            // Act
            var result = await _groupController.Delete(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task StudentGroupController_Delete_HttpGet_GroupNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            StudentGroup group = null;
            A.CallTo(() => _groupRepository.GetByIdAsync(id)).Returns(Task.FromResult(group));

            // Act
            var result = await _groupController.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task StudentGroupController_Delete_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var group = new StudentGroup { GroupName = "Test", ProgramID = 1 };
            A.CallTo(() => _groupRepository.GetByIdAsync(id)).Returns(Task.FromResult(group));

            // Act
            var result = await _groupController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task StudentGroupController_Delete_HttpPost_GroupNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            StudentGroup group = null;
            A.CallTo(() => _groupRepository.GetByIdAsync(id)).Returns(Task.FromResult(group));

            // Act
            var result = await _groupController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
