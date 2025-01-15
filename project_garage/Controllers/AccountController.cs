using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IRepository;

namespace project_garage.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailSender _emailSender;

        public AccountController(IUserRepository userRepository, IAuthRepository authRepository)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
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
            Console.WriteLine("entered regisrt");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("im invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"{error.ErrorMessage}");
                }
                return View(model);
            }

            var emailCheck = _userRepository.GetByEmailAsync(model.Email);
            if (emailCheck != null)
            {
                ModelState.AddModelError(string.Empty, "Email зайнятий");
                return View(model);
            }

            Console.WriteLine("i got data");


            var user = new UserModel
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmationCode = Guid.NewGuid().ToString()
            };
            var result = await _userRepository.CreateUserAsync(user, model.Password);

            if (result.Succeeded)
            {
                Console.WriteLine("succed");
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, code = user.EmailConfirmationCode }, Request.Scheme);

                await _emailSender.SendEmailAsync(model.Email, "Підтвердження email",
                    $"Перейдіть за посиланням для підтвердження акаунта: <a href='{confirmationLink}'>посилання</a>");
                Console.WriteLine("sended");
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Недійсний запит.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдено.");
            }

            if (user.EmailConfirmationCode != code)
            {
                return View("InvalidCodeEmail");
            }

            
            var result = await _userRepository.ConfirmUserEmail(user);

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
            var user = await _userRepository.GetByEmailAsync(model.Email);
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

            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, model.Password);
            Console.WriteLine($"IsPasswordValid: {isPasswordValid}");
            if (isPasswordValid)
            {
                await _authRepository.SignInAsync(user);
                return RedirectToAction("ProfileIndex", "Profile", new { id = user.Id });
            }

            Console.WriteLine("Smth going wrong");
            ModelState.AddModelError(string.Empty, "Невдала спроба входу. Перевірте дані.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _authRepository.SignOutAsync(); // Викликає розлогування
            return RedirectToAction("Login", "Account"); // Перенаправлення на головну сторінку або іншу
        }
    }
}