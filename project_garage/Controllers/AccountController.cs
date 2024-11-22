using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_garage.Models;

namespace project_garage.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserModel> _userManager;

        public AccountController(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            Console.WriteLine("Molly");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserModel model, string password)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            var user = new UserModel
            {
                UserName = model.UserName,
                Email = model.Email,
            };
            Console.WriteLine($"{user.UserName}");
            Console.WriteLine($"{user.Email}");
            Console.WriteLine($"{password}");
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}
