using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudentMedicalCertificateSystem.Controllers;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentMedicalCertificateSystem.Tests.Controllers
{
    public class AdminControllerTests
    {
        private AdminController _adminController;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdminControllerTests()
        {
            //Dependencies
            _userManager = A.Fake<UserManager<User>>();
            _roleManager = A.Fake<RoleManager<IdentityRole>>();

            //SUT
            _adminController = new AdminController(_userManager, _roleManager);
        }

        [Fact]
        public void AdminController_Index_ReturnsSuccess()
        {
            // Arrange
            var users = new List<User> { new User { UserName = "user1" }, new User { UserName = "user2" } };
            var roles = new List<string> { "Role1", "Role2" };

            A.CallTo(() => _userManager.Users).Returns(users.AsQueryable());
            A.CallTo(() => _userManager.GetRolesAsync(A<User>._)).Returns(roles);

            // Act
            var result = _adminController.Index();

            // Assert
            result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<List<UserViewModel>>();
        }

        [Fact]
        public async Task AdminController_CreateUser_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            var allRoles = new List<IdentityRole> { new IdentityRole("Role1"), new IdentityRole("Role2") };
            A.CallTo(() => _roleManager.Roles).Returns(allRoles.AsQueryable());

            // Act
            var result = await _adminController.CreateUser();

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.Model.Should().BeOfType<CreateUserViewModel>()
                .Which.AllRoles.Should().HaveCount(2);
        }

        [Fact]
        public async Task AdminController_CreateUser_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var allRoles = new List<IdentityRole> { new IdentityRole("Role1"), new IdentityRole("Role2") };
            var viewModel = new CreateUserViewModel 
            { 
                UserName = null,
                Password = "password",
                ConfirmPassword = "password",
                LastName = "Test",
                FirstName = "Test",
                Patronymic = "Test",
                UserRole = "Test",
                AllRoles = allRoles
            };
            A.CallTo(() => _roleManager.Roles).Returns(allRoles.AsQueryable());
            _adminController.ModelState.AddModelError("UserName", "");
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            _adminController.TempData = new TempDataDictionary(httpContext, tempDataProvider);

            // Act
            var result = await _adminController.CreateUser(viewModel);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.TempData["Error"].Should().Be("Ошибка");
        }

        [Fact]
        public async Task AdminController_CreateUser_HttpPost_UserAlreadyExists_ReturnsFailure()
        {
            // Arrange
            string userName = "TEST"; 
            var user = new User { UserName = userName };
            var allRoles = new List<IdentityRole> { new IdentityRole("Role1"), new IdentityRole("Role2") };
            var viewModel = new CreateUserViewModel
            {
                UserName = userName,
                Password = "password",
                ConfirmPassword = "password",
                LastName = "Test",
                FirstName = "Test",
                Patronymic = "Test",
                UserRole = "Test",
                AllRoles = allRoles
            };

            A.CallTo(() => _roleManager.Roles).Returns(allRoles.AsQueryable());
            A.CallTo(() => _userManager.FindByNameAsync(userName)).Returns(user);
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            _adminController.TempData = new TempDataDictionary(httpContext, tempDataProvider);

            // Act
            var result = await _adminController.CreateUser(viewModel);

            // Assert
            result.Should().BeOfType<ViewResult>()
                .Which.TempData["Error"].Should().Be("Пользователь с данным именем уже существует");
        }

        [Fact]
        public async Task AdminController_CreateUser_HttpPost_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            string userName = "TEST1234";
            User user = null;
            var allRoles = new List<IdentityRole> { new IdentityRole("Role1"), new IdentityRole("Role2") };
            var viewModel = new CreateUserViewModel
            {
                UserName = userName,
                Password = "password1",
                ConfirmPassword = "password2",
                LastName = "Test",
                FirstName = "Test",
                Patronymic = "Test",
                UserRole = "Test",
                AllRoles = allRoles
            };

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var options = Options.Create(new IdentityOptions());
            var userValidators = new List<IUserValidator<User>>();
            var passwordValidators = new List<IPasswordValidator<User>> { new PasswordValidator<User>() }; 
            var userManager = new UserManager<User>(
                new UserStore<User>(dbContext),
                options,
                new PasswordHasher<User>(),
                userValidators,
                passwordValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                new Logger<UserManager<User>>(new LoggerFactory()));


            A.CallTo(() => _roleManager.Roles).Returns(allRoles.AsQueryable());
            A.CallTo(() => _userManager.FindByNameAsync(userName)).Returns(user);
            var adminController = new AdminController(userManager, _roleManager);

            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            adminController.TempData = new TempDataDictionary(httpContext, tempDataProvider);


            // Act
            var result = await adminController.CreateUser(viewModel);

            // Assert
            result.Should().BeOfType<ViewResult>()
        .Which.TempData["Error"].Should().Be("Пароль не соответствует требованиям безопасности." +
            " Минимальная длина пароля - 6 символов, он должен содержать символы " +
            "нижнего и верхнего регистра, цифры, а также специальные символы.");
        }

        [Fact]
        public async Task AdminController_CreateUser_HttpPost_ValidUser_ReturnsSuccess()
        {
            // Arrange
            string userName = "TEST";
            User user = null;
            var allRoles = new List<IdentityRole> { new IdentityRole("Test"), new IdentityRole("Role2") };
            var viewModel = new CreateUserViewModel
            {
                UserName = userName,
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                LastName = "Test",
                FirstName = "Test",
                Patronymic = "Test",
                UserRole = "TEST",
                AllRoles = allRoles
            };

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var testRole = new IdentityRole("TEST");
            await dbContext.Roles.AddAsync(testRole);
            await dbContext.SaveChangesAsync();
            var options = Options.Create(new IdentityOptions());
            var userValidators = new List<IUserValidator<User>>();
            var passwordValidators = new List<IPasswordValidator<User>> { new PasswordValidator<User>() };
            var userManager = new UserManager<User>(
                new UserStore<User>(dbContext),
                options,
                new PasswordHasher<User>(),
                userValidators,
                passwordValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                new Logger<UserManager<User>>(new LoggerFactory()));

            
            var roleStore = new RoleStore<IdentityRole>(dbContext);
            var roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);
            await roleManager.CreateAsync(new IdentityRole("TEST"));

           
            var adminController = new AdminController(userManager, roleManager);

            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            adminController.TempData = new TempDataDictionary(httpContext, tempDataProvider);

            // Act
            var result = await adminController.CreateUser(viewModel);

            // Assert
            var viewResult = result as RedirectToActionResult;
            viewResult.Should().NotBeNull();
            viewResult.Should().BeOfType<RedirectToActionResult>();
            viewResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task AdminController_Edit_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            string id = "1";
            var user = new User { Id = id, UserName = "testuser" };
            var userRoles = new List<string> { "Role1", "Role2" };
            var allRoles = new List<IdentityRole> { new IdentityRole("Role1"), new IdentityRole("Role2") };
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(user);
            A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(userRoles);
            A.CallTo(() => _roleManager.Roles).Returns(allRoles.AsQueryable());

            // Act
            var result = await _adminController.Edit(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<ChangeRoleViewModel>();
            var model = viewResult.Model as ChangeRoleViewModel;
            model.UserName.Should().Be(user.UserName);
            model.UserID.Should().Be(user.Id);
            model.UserRole.Should().Be(userRoles[0]);
            model.AllRoles.Should().BeEquivalentTo(allRoles);
        }

        [Fact]
        public async Task AdminController_Edit_HttpGet_InvalidUserId_ReturnsNotFound()
        {
            // Arrange
            string id = "-5555";
            User user = null;
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(user);

            // Act
            var result = await _adminController.Edit(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AdminController_Edit_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            string id = "1";
            string role = "Role1";
            var user = new User { Id = id, UserName = "testuser" };
            var userRoles = new List<string> { "Role2" };
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(user);
            A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(userRoles);

            // Act
            var result = await _adminController.Edit(id, role);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            A.CallTo(() => _userManager.RemoveFromRolesAsync(user, userRoles)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _userManager.AddToRoleAsync(user, role)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AdminController_Edit_HttpPost_InvalidUserId_ReturnsNotFound()
        {
            // Arrange
            string id = "999"; 
            string role = "Role1";
            User user = null;
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(user);

            // Act
            var result = await _adminController.Edit(id, role);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AdminController_Delete_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            string id = "id";
            var user = new User();
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(Task.FromResult(user));

            // Act
            var result = await _adminController.Delete(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task AdminController_Delete_HttpGet_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            string id = "id";
            User user = null;
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(Task.FromResult(user));

            // Act
            var result = await _adminController.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AdminController_Delete_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            string id = "id";
            var user = new User();
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(Task.FromResult(user));

            // Act
            var result = await _adminController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectToActionResult = result as RedirectToActionResult;
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task AdminController_Delete_HttpPost_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            string id = "id";
            User user = null;
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(Task.FromResult(user));

            // Act
            var result = await _adminController.DeleteConfirmed(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AdminController_ChangePassword_HttpGet_ValidData_ReturnsSuccess()
        {
            // Arrange
            string id = "1";
            var user = new User { Id = id, UserName = "testuser" };
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(user);

            // Act
            var result = await _adminController.ChangePassword(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<ChangePasswordViewModel>();
        }

        [Fact]
        public async Task AdminController_ChangePassword_HttpGet_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            string id = "1";
            User user = null;
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(user);

            // Act
            var result = await _adminController.ChangePassword(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AdminController_ChangePassword_HttpPost_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            string userName = "TEST123456";
            User user = new User { Id = "id1234"};
            var allRoles = new List<IdentityRole> { new IdentityRole("Role1"), new IdentityRole("Role2") };
            var viewModel = new ChangePasswordViewModel
            {
                UserId = "id1234",
                UserName = userName,
                Password = "password1",
                ConfirmPassword = "password1"
            };

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
          
            var options = Options.Create(new IdentityOptions());
            var userValidators = new List<IUserValidator<User>>();
            var passwordValidators = new List<IPasswordValidator<User>> { new PasswordValidator<User>() };
            var userManager = new UserManager<User>(
                new UserStore<User>(dbContext),
                options,
                new PasswordHasher<User>(),
                userValidators,
                passwordValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                new Logger<UserManager<User>>(new LoggerFactory()));
            await userManager.CreateAsync(user);
            var adminController = new AdminController(userManager, _roleManager);
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            adminController.TempData = new TempDataDictionary(httpContext, tempDataProvider);


            // Act
            var result = await adminController.ChangePassword(viewModel);

            // Assert
            result.Should().BeOfType<ViewResult>()
        .Which.TempData["Error"].Should().Be("Пароль не соответствует требованиям безопасности." +
                    " Минимальная длина пароля - 6 символов, он должен содержать символы " +
                    "нижнего и верхнего регистра, цифры, а также специальные символы.");
        }

        [Fact]
        public async Task AdminController_ChangePassword_HttpPost_UserNotFound_ReturnsFailure()
        {
            // Arrange
            string id = "id12345";
            User user = null;
            var viewModel = new ChangePasswordViewModel
            {
                UserId = "id12345",
                UserName = "test",
                Password = "password1",
                ConfirmPassword = "password1"
            };
            A.CallTo(() => _userManager.FindByIdAsync(id)).Returns(user);

            // Act
            var result = await _adminController.ChangePassword(viewModel);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AdminController_ChangePassword_HttpPost_ValidData_ReturnsSuccess()
        {
            // Arrange
            string userId = "id12348";
            string userName = "TEST123456";
            var user = new User { Id = userId, UserName = userName };
            var newPassword = "NewPassword123!";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var userValidators = new List<IUserValidator<User>>();
            var passwordValidators = new List<IPasswordValidator<User>> { new PasswordValidator<User>() };
            var userManager = new UserManager<User>(
                new UserStore<User>(dbContext),
                null,
                new PasswordHasher<User>(),
                userValidators,
                passwordValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                new Logger<UserManager<User>>(new LoggerFactory()));

            var dataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"path\to\keys-directory"));
            var options2 = Options.Create(new DataProtectionTokenProviderOptions());
            userManager.RegisterTokenProvider("Default", new DataProtectorTokenProvider<User>(
                dataProtectionProvider.CreateProtector("ASP.NET Identity"),
                options2,
                new Logger<DataProtectorTokenProvider<User>>(new LoggerFactory())));

            var viewModel = new ChangePasswordViewModel
            {
                UserId = userId,
                UserName = userName,
                Password = newPassword,
                ConfirmPassword = newPassword
            };

            var adminController = new AdminController(userManager, _roleManager);
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = A.Fake<ITempDataProvider>();
            adminController.TempData = new TempDataDictionary(httpContext, tempDataProvider);

            // Act
            var result = await adminController.ChangePassword(viewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            var changedUser = await userManager.FindByIdAsync(userId);
            var passwordChanged = await userManager.CheckPasswordAsync(changedUser, newPassword);
            passwordChanged.Should().BeTrue();
        }

        [Fact]
        public async Task AdminController_ChangePassword_HttpPost_InvalidModelState_ReturnsFailure()
        {
            // Arrange
            var viewModel = new ChangePasswordViewModel
            {
                UserId = "id12345",
                UserName = "test",
                Password = null,
                ConfirmPassword = "password1"
            };
            _adminController.ModelState.AddModelError("Password", "");

            // Act
            var result = await _adminController.ChangePassword(viewModel);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<ChangePasswordViewModel>();
            _adminController.ModelState.IsValid.Should().BeFalse();
        }


    }
}
