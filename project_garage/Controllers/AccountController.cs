using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Models;

namespace project_garage.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
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

            Console.WriteLine("i got data");


            var user = new UserModel
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmationCode = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, model.Password);

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

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдено.");
            }

            if (user.EmailConfirmationCode != code)
            {
                return BadRequest("Недійсний код підтвердження.");
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = "none"; // Видаляємо код після успішного підтвердження
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return View("EmailConfirmed"); // Сторінка успішного підтвердження
            }

            return BadRequest("Щось пішло не так.");
        }

    }
}
