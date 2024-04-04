using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using StudentMedicalCertificateSystem.Controllers;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Controllers
{
    public class AccountControllerTests
    {
        private AccountController _accountController;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public AccountControllerTests()
        {
            //Dependencies
            _userManager = A.Fake<UserManager<User>>();
            _signInManager = A.Fake<SignInManager<User>>();

            //SUT
            _accountController = new AccountController(_userManager, _signInManager);
        }

        [Fact]
        public async Task AccountController_Login_HttpGet_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = _accountController.Login();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task AccountController_Login_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            var viewModel = new LoginViewModel
            {
                Login = "username",
                Password = "password"
            };
            var user = new User { UserName = "username" };

            A.CallTo(() => _userManager.FindByNameAsync(viewModel.Login)).Returns(user);
            A.CallTo(() => _userManager.CheckPasswordAsync(user, viewModel.Password)).Returns(true);
            A.CallTo(() => _signInManager.PasswordSignInAsync(user, viewModel.Password, false, false))
                .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

            // Act
            var result = await _accountController.Login(viewModel);

            // Assert
            var viewResult = result as RedirectToActionResult;
            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<RedirectToActionResult>();
            viewResult.ActionName.Should().Be("Index");
            viewResult.ControllerName.Should().Be("MedicalCertificate");
        }

        [Fact]
        public async Task AccountController_Login_HttpPost_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            var viewModel = new LoginViewModel
            {
                Login = "username",
                Password = "password"
            };
            var user = new User { UserName = "username", PasswordHash = "hash" };

            A.CallTo(() => _userManager.FindByNameAsync(viewModel.Login)).Returns(user);
            A.CallTo(() => _userManager.CheckPasswordAsync(user, viewModel.Password)).Returns(false);
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            _accountController.TempData = new TempDataDictionary(httpContext, tempDataProvider);

            // Act
            var result = await _accountController.Login(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<ViewResult>();
            viewResult.Model.Should().Be(viewModel);
            _accountController.TempData["Error"].Should().Be("Неверный пароль. Пожалуйста, попробуйте еще раз");
        }

        [Fact]
        public async Task AccountController_Login_HttpPost_InvalidUserName_ReturnsFailure()
        {
            // Arrange
            var viewModel = new LoginViewModel
            {
                Login = "username",
                Password = "password"
            };
            User user = null;
            A.CallTo(() => _userManager.FindByNameAsync(viewModel.Login)).Returns(user);
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            _accountController.TempData = new TempDataDictionary(httpContext, tempDataProvider);

            // Act
            var result = await _accountController.Login(viewModel);

            // Assert
            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<ViewResult>();
            viewResult.Model.Should().Be(viewModel);
            _accountController.TempData["Error"].Should().Be("Пользователь с данным логином не найден. Попробуйте еще раз");
        }

        [Fact]
        public async Task AccountController_Login_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var viewModel = new LoginViewModel
            {
                Login = null,
                Password = "password"
            };
            _accountController.ModelState.AddModelError("Login", "");

            // Act
            var result = await _accountController.Login(viewModel);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<LoginViewModel>();
            _accountController.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task AccountController_Logout_ReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _accountController.Logout();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Login");
            (result as RedirectToActionResult).ControllerName.Should().Be("Account");

            A.CallTo(() => _signInManager.SignOutAsync()).MustHaveHappenedOnceExactly();
        }
    }
}
