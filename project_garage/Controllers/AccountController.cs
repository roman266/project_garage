using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace project_garage.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public AccountController(IUserService userService, IAuthService authService, IJwtService jwtService)
        {
            _userService = userService;
            _authService = authService;
            _jwtService = jwtService;
        }

        private IActionResult JsonResponse(object data, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            return Json(data);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Повертаємо порожній JSON або об'єкт, який містить помилки
                return JsonResponse(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() }, 400);
            }

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var result = await _userService.CreateUserAsync(model.UserName, model.Email, model.Password, baseUrl);
                return JsonResponse(new { success = true, message = "Email confirmation code has been sended on your email" });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpPost]
        [Route("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                await _userService.ConfirmUserEmail(userId, code);
                return JsonResponse(new { success = true, message = "Email successfully confirmed" }); // Сторінка успішного підтвердження
            }
            catch (Exception ex) {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponse(new
                {
                    success = false,
                    message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                }, 400);
            }
            
            try
            {
                var user = await _authService.SignInAsync(model.Email, model.Password);
                if (user == null)
                {
                    return JsonResponse(new { success = false, message = "Invalid email or password." }, 401);
                }

                // Генерація JWT-токена
                var token = _jwtService.GenerateToken(user.Id, user.Email);

                // Налаштування cookie з токеном
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Захист від XSS
                    Secure = true,   // Працює тільки через HTTPS
                    SameSite = SameSiteMode.None, // Захист від CSRF
                    Expires = DateTime.UtcNow.AddHours(1) // Термін дії токена
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);

                return JsonResponse(new
                {
                    success = true,
                    userId = user.Id,
                    message = "You successfully logged in"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("AuthToken");
                return JsonResponse(new { success = true, message = "Logged out" }, 200);
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }
    }
}