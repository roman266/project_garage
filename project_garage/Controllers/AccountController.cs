using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace project_garage.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;

        public AccountController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
            _emailSender = new EmailSender();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            var emailCheck = _userService.GetByEmailAsync(model.Email);
            if (emailCheck != null)
            {
                ModelState.AddModelError(string.Empty, "Email зайнятий");
                return Json(new { success = false, message = "Email зайнятий" });
            }

            Console.WriteLine("i got data");


            var user = new UserModel
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmationCode = Guid.NewGuid().ToString()
            };
            var result = await _userService.CreateUserAsync(user, model.Password);

            if (result.Succeeded)
            {
                Console.WriteLine("succed");
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, code = user.EmailConfirmationCode }, Request.Scheme);

                await _emailSender.SendEmailAsync(model.Email, "Підтвердження email",
                    $"Перейдіть за посиланням для підтвердження акаунта: <a href='{confirmationLink}'>посилання</a>");
                Console.WriteLine("sended");
                return Json(new { success = true, message = "Реєстрація успішна. Перевірте вашу пошту для підтвердження акаунта." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            var errors = result.Errors.Select(e => e.Description).ToList();

            return Json(new { success = false, errors });
        }

        [HttpGet]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine("Login POST called");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("invalid");
                return View(model);
            }
            Console.WriteLine(model.Email);
            // Отримуємо користувача за email
            var user = await _userService.GetByEmailAsync(model.Email);
            if (user == null)
            {
                Console.WriteLine("there's no users with this email");
                ModelState.AddModelError(string.Empty, "Невдала спроба входу. Перевірте дані.");
                return View(model);
            }

            // Перевірка email-верифікації
            if (!user.IsEmailConfirmed)
            {
                Console.WriteLine("email doesn't confirmed");
                ModelState.AddModelError(string.Empty, "Ваш email не підтверджений. Перевірте пошту для активації.");
                return View(model);
            }

            var isPasswordValid = await _userService.CheckPasswordAsync(user, model.Password);
            Console.WriteLine($"IsPasswordValid: {isPasswordValid}");
            if (isPasswordValid)
            {
                await _authService.SignInAsync(user);
                return RedirectToAction("ProfileIndex", "Profile", new { id = user.Id });
            }

            Console.WriteLine("Smth going wrong");
            ModelState.AddModelError(string.Empty, "Невдала спроба входу. Перевірте дані.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync(); // Викликає розлогування
            return RedirectToAction("Login", "Account"); // Перенаправлення на головну сторінку або іншу
        }
    }
}