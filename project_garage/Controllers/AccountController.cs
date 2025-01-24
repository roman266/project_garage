using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using System.Linq.Expressions;

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

        private IActionResult JsonResponse(object data, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponse(new { success = false, 
                    message = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() }, 500);
            }

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                await _userService.CreateUserAsync(model.UserName, model.Email, model.Password, baseUrl);
                return JsonResponse(new { success = true, message = "Email confirmation code has been sended on your email" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.Message}" }, 400);
            }
        }

        [HttpPost]
        [Route("Account/EmailConfirmed/{userId}")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                await _userService.ConfirmUserEmail(userId, code);
                return Json(new { success = true, message = "Email successfully confirmed" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (InvalidDataException ex)
            {
                return Json(new { success = false, message = ex.Message }, 500);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, 400);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponse(new { success = false, message = "Invalid model" }, 500);
            }

            try
            {
                var profileIndex = await _authService.SignInAsync(model.Email, model.Password);
                return JsonResponse(new { success = true, message = "You successfully loged", userId = profileIndex});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.Message}" }, 400);
            }
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                await _authService.SignOutAsync(); // Викликає розлогування
                return Json(new { success = false, message = "You successfully loged out" }); // Перенаправлення на головну сторінку або іншу
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, 400);
            }
        }
    }
}