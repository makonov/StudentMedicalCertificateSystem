using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Interfaces;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.Repository;
using StudentMedicalCertificateSystem.ViewModels;

namespace StudentMedicalCertificateSystem.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IEducationalProgramRepository officeRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                var viewModel = new UserViewModel
                {
                    UserID = user.Id,
                    UserName = user.UserName,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    Patronymic = user.Patronymic,
                    Roles = roles.ToList()
                };
                userViewModels.Add(viewModel);
            }
            return View(userViewModels);
        }

        public async Task<IActionResult> CreateUser()
        {
            var allRoles = _roleManager.Roles.ToList();
            var response = new CreateUserViewModel
            {
                AllRoles = allRoles
            };

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel createUserViewModel)
        {
            if (!ModelState.IsValid) 
            {
                TempData["Error"] = "Ошибка";
                createUserViewModel.AllRoles = _roleManager.Roles.ToList();
                return View(createUserViewModel);
            } 

            var user = await _userManager.FindByNameAsync(createUserViewModel.UserName);
            if (user != null)
            {
                TempData["Error"] = "Пользователь с данным именем уже существует";
                createUserViewModel.AllRoles = _roleManager.Roles.ToList();
                return View(createUserViewModel);
            }

            // Проверка соответствия пароля требованиям
            var passwordValidationResult = await _userManager.PasswordValidators.First().ValidateAsync(_userManager, null, createUserViewModel.Password);
            if (!passwordValidationResult.Succeeded)
            {
                // Пароль не соответствует требованиям
                TempData["Error"] = "Пароль не соответствует требованиям безопасности." +
                    " Минимальная длина пароля - 6 символов, он должен содержать символы " +
                    "нижнего и верхнего регистра, цифры, а также специальные символы.";
                createUserViewModel.AllRoles = _roleManager.Roles.ToList();
                return View(createUserViewModel);
            }

            var newUser = new User
            {
                UserName = createUserViewModel.UserName,
                LastName = createUserViewModel.LastName,
                FirstName = createUserViewModel.FirstName,
                Patronymic = createUserViewModel.Patronymic
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, createUserViewModel.Password);

            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, createUserViewModel.UserRole);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string userId)
        {
            // получаем пользователя
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserName = user.UserName,
                    UserID = user.Id,
                    UserRole = userRoles[0],
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string userId, string role)
        {
            // получаем пользователя
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var currentRole = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRole);
                await _userManager.AddToRoleAsync(user, role);

                return RedirectToAction("Index");
            }

            return NotFound();
        }

        public async Task<IActionResult> Delete(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return View(user);
            }

            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            User? user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangePassword(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                ChangePasswordViewModel viewModel = new ChangePasswordViewModel{ UserName = user.UserName, UserId = userId };
                return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            // Проверка соответствия пароля требованиям
            var passwordValidationResult = await _userManager.PasswordValidators.First().ValidateAsync(_userManager, null, viewModel.Password);
            if (!passwordValidationResult.Succeeded)
            {
                // Пароль не соответствует требованиям
                TempData["Error"] = "Пароль не соответствует требованиям безопасности." +
                    " Минимальная длина пароля - 6 символов, он должен содержать символы " +
                    "нижнего и верхнего регистра, цифры, а также специальные символы.";

                return View(viewModel);
            }

            User? user = await _userManager.FindByIdAsync(viewModel.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, viewModel.Password);
            
            if (result.Succeeded)
                TempData["Success"] = "Пароль успешно изменен.";
            else
                TempData["Error"] = "Не удалось изменить пароль.";

            return RedirectToAction("Index");
        }
    }
}
