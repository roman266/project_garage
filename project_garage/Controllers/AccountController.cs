﻿using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;

namespace project_garage.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        

        public AccountController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
            
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Виведення помилок у консоль
                Console.WriteLine("Model is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"{error.ErrorMessage}");
                }

                // Повертаємо порожній JSON або об'єкт, який містить помилки
                return Json(new { success = false, 
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            Console.WriteLine("i got data");

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var result = await _userService.CreateUserAsync(model.UserName, model.Email, model.Password, baseUrl);
                return Json(new { success = true, message = "Email confirmation code has been sended on your email" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.Message}" });
            }
        }

        [HttpGet]
        [Route("Account/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Недійсний запит.");
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдено.");
            }

            if (user.EmailConfirmationCode != code)
            {
                return View("InvalidCodeEmail");
            }

            
            var result = await _userService.ConfirmUserEmail(user);

            if (result.Succeeded)
            {
                return View("EmailConfirmed"); // Сторінка успішного підтвердження
            }

            return BadRequest("Щось пішло не так.");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine("Login POST called");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("invalid");
                return View(model);
            }
            Console.WriteLine(model.Email);

            try
            {
                var profileIndex = await _authService.SignInAsync(model.Email, model.Password);
                return Json(new { success = true, userId = profileIndex, message = "You successfully loged" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.Message}" });
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync(); // Викликає розлогування
            return Json(new { success = false, message = "You successfully loged out" }); // Перенаправлення на головну сторінку або іншу
        }
    }
}