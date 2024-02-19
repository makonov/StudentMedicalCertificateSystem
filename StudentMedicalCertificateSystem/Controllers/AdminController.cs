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
        private readonly ApplicationDbContext _context;
        private readonly IEducationalOfficeRepository _officeRepository;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IEducationalOfficeRepository officeRepository)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _officeRepository = officeRepository;
        }

        public IActionResult Index()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            var users = _userManager.Users.Where(u => u.Id != currentUser.Id).ToList();
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
            ViewBag.OfficeList = new SelectList(await GetOffices(), "Value", "Text");
            return View(response);
        }

        private async Task<List<SelectListItem>> GetOffices()
        {
            // Здесь получаем список офисов из базы данных
            return await _officeRepository.GetOfficesAsSelectList();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel createUserViewModel)
        {
            if (!ModelState.IsValid) 
            {
                TempData["Error"] = "Ошибка";
                ViewBag.OfficeList = new SelectList(await GetOffices(), "Value", "Text");
                createUserViewModel.AllRoles = _roleManager.Roles.ToList();
                return View(createUserViewModel);
            } 

            var user = await _userManager.FindByNameAsync(createUserViewModel.UserName);
            if (user != null)
            {
                TempData["Error"] = "Пользователь с данным именем уже существует";
                ViewBag.OfficeList = new SelectList(await GetOffices(), "Value", "Text");
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
                ViewBag.OfficeList = new SelectList(await GetOffices(), "Value", "Text");
                createUserViewModel.AllRoles = _roleManager.Roles.ToList();
                return View(createUserViewModel);
            }

            var newUser = new User
            {
                UserName = createUserViewModel.UserName,
                LastName = createUserViewModel.LastName,
                FirstName = createUserViewModel.FirstName,
                Patronymic = createUserViewModel.Patronymic,
                OfficeID = (int) createUserViewModel.OfficeID
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
    }
}
