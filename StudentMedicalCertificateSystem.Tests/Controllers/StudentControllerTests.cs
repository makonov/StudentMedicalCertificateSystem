using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Controllers;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;
using StudentMedicalCertificateSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Controllers
{
    public class StudentControllerTests
    {
        private StudentController _studentController;
        private IStudentRepository _studentRepository;
        private IStudentGroupRepository _groupRepository;
        private IEducationalProgramRepository _programRepository;

        public StudentControllerTests()
        {
            //Dependencies
            _studentRepository = A.Fake<IStudentRepository>();
            _groupRepository = A.Fake<IStudentGroupRepository>();
            _programRepository = A.Fake<IEducationalProgramRepository>();

            //SUT
            _studentController = new StudentController(_studentRepository, _groupRepository, _programRepository);
        }

        [Fact]
        public async Task StudentController_Index_ReturnsSuccess()
        {
            //Arrange
            var expectedStudents = new List<Student> { new Student(), new Student() };
            var expectedViewModel = new ShowStudentsViewModel() { Students = expectedStudents };
            A.CallTo(() => _studentRepository.Count()).Returns(5);
            A.CallTo(() => _studentRepository.GetPagedStudentsAsync(A<int>._, A<int>._)).Returns(Task.FromResult(expectedStudents));

            //Act
            var result = await _studentController.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<ShowStudentsViewModel>();
        }

        [Fact]
        public async Task StudentController_Create_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var viewModel = new CreateStudentViewModel
            {
                GroupID = 5,
                LastName = "LastName",
                FirstName = "FirstName",
                Patronymic = "Patronymic",
                Course = 1,
                BirthDate = DateTime.Now,
                IsFromCertificate = false
            };

            // Act
            var result = await _studentController.Create(viewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task StudentController_Create_HttpPost_FromCertificate_ValidData_ReturnsSuccess()
        {
            // Arrange
            var viewModel = new CreateStudentViewModel
            {
                GroupID = 5,
                LastName = "LastName",
                FirstName = "FirstName",
                Patronymic = "Patronymic",
                Course = 1,
                BirthDate = DateTime.Now,
                IsFromCertificate = true
            };

            // Act
            var result = await _studentController.Create(viewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ControllerName.Should().Be("MedicalCertificate");
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Create");
        }

        [Fact]
        public async Task StudentController_Create_HttpPost_InvalidModelState_ReturnsFailure()
        {
           //Arrange
           var viewModel = new CreateStudentViewModel
           {
               GroupID = 1,
               LastName = null,
               FirstName = "FirstName",
               Patronymic = "Patronymic",
               Course = 1,
               BirthDate = null,
               IsFromCertificate = false
           };
            _studentController.ModelState.AddModelError("LastName", "Поле 'Фамилия' обязательно для заполнения");

            // Act
            var result = await _studentController.Create(viewModel);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<CreateStudentViewModel>();
            _studentController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void StudentController_Create_HttpGet_ReturnSuccess()
        {
            //Arrange

            //Act
            var result = _studentController.Create();

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();

        }

        [Fact]
        public void StudentController_Edit_HttpGet_ReturnsSuccess()
        {
            //Arrange
            var id = 1;
            var student = A.Fake<Student>();
            A.CallTo(() => _studentRepository.GetIncludedByIdAsync(id)).Returns(student);

            var viewModel = new EditStudentViewModel
            {
                StudentID = student.StudentID,
                GroupID = student.GroupID,
                LastName = student.LastName,
                FirstName = student.FirstName,
                Patronymic = student.Patronymic,
                Course = student.Course,
                BirthDate = student.BirthDate
            };

            //Act
            var result = _studentController.Edit(id);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();

        }

        [Fact]
        public async Task StudentController_Edit_HttpGet_StudentNotFound_ReturnsNotFound()
        {
            //Arrange
            var id = 1;
            Student student = null;
            A.CallTo(() => _studentRepository.GetIncludedByIdAsync(id)).Returns(student);

            var viewModel = new EditStudentViewModel();

            //Act
            var result = await _studentController.Edit(id);

            //Assert
            result.Should().BeOfType<NotFoundResult>();

        }

        [Fact]
        public async Task StudentController_Edit_HttpPost_ValidData_ReturnSuccess()
        {
            // Arrange
            var viewModel = new EditStudentViewModel
            {
                StudentID = 2,
                GroupID = 2,
                LastName = "LastName",
                FirstName = "FirstName",
                Patronymic = "Patronymic",
                Course = 1,
                BirthDate = DateTime.Now
            };

            // Act
            var result = await _studentController.Edit(1, viewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task StudentController_Edit_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var viewModel = new EditStudentViewModel
            {
                StudentID = 2,
                GroupID = 2,
                LastName = null,
                FirstName = "FirstName",
                Patronymic = "Patronymic",
                Course = 1,
                BirthDate = DateTime.Now
            };
            _studentController.ModelState.AddModelError("LastName", "Поле 'Фамилия' обязательно для заполнения");

            // Act
            var result = await _studentController.Edit(1, viewModel);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<EditStudentViewModel>();
            _studentController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task StudentController_Delete_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var student = new Student();
            A.CallTo(() => _studentRepository.GetIncludedByIdAsync(id)).Returns(Task.FromResult(student));

            // Act
            var result = await _studentController.Delete(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task StudentController_Delete_HttpGet_StudentNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            Student student = null;
            A.CallTo(() => _studentRepository.GetIncludedByIdAsync(id)).Returns(Task.FromResult(student));

            // Act
            var result = await _studentController.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task StudentController_Delete_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            var student = new Student();
            A.CallTo(() => _studentRepository.GetIncludedByIdAsync(id)).Returns(Task.FromResult(student));

            // Act
            var result = await _studentController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task StudentController_Delete_HttpPost_StudentNotFound_ReturnsNotFound()
        {
            // Arrange
            int id = -5;
            Student student = null;
            A.CallTo(() => _studentRepository.GetIncludedByIdAsync(id)).Returns(Task.FromResult(student));

            // Act
            var result = await _studentController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task StudentController_Find_ValidData_ReturnsSuccess()
        {
            //Arrange
            var viewModel = new FindStudentViewModel { StudentData = "DATA DATA DATA - DATA"};
            var student = A.Fake<Student>();
            A.CallTo(() => _studentRepository.GetByFullNameAndGroupAsync(A<string>._, A<string>._, A<string>._, A<string>._)).Returns(Task.FromResult(student));

            //Act
            var result = await _studentController.Find(viewModel) as ViewResult;

            //Assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be("Index");
            result.Model.Should().BeOfType<ShowStudentsViewModel>();
        }

        [Fact]
        public async Task StudentController_Find_InvalidModelState_ReturnsFailure()
        {
            //Arrange
            var viewModel = new FindStudentViewModel { StudentData = null };
            _studentController.ModelState.AddModelError("StudentData", "Поле 'Фамилия' обязательно для заполнения");

            //Act
            var result = await _studentController.Find(viewModel) as ViewResult;

            //Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<FindStudentViewModel>();
            _studentController.ModelState.IsValid.Should().BeFalse();
        }
    }
}
