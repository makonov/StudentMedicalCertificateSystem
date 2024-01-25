using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentMedicalCertificateSystem.Data;
using StudentMedicalCertificateSystem.Models;
using StudentMedicalCertificateSystem.ViewModels;

namespace StudentMedicalCertificateSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByNameAsync(loginViewModel.Login);

            if (user != null)
            {
                // Пользователь найден, проверка пароля
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if(passwordCheck)
                {
                    // Пароль верный, авторизация
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "MedicalCertificate");
                    }
                }
                // Пароль неверный 
                TempData["Error"] = "Неверный пароль. Пожалуйста, попробуйте еще раз";
                return View(loginViewModel);
            }
            // Пользователь не найден
            TempData["Error"] = "Пользователь с данным логином не найден. Попробуйте еще раз";
            return View(loginViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
