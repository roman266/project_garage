using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;

namespace project_garage.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto model)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var result = await _userService.CreateUserAsync(model.UserName, model.Email, model.Password, baseUrl);
                return Ok(new { message = "Email confirmation code has been sended on your email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                await _userService.ConfirmUserEmail(userId, code);
                return Ok(new { message = "Email successfully confirmed" });
            }
            catch (Exception ex) {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto model)
        {
            try
            {
                var user = await _authService.SignInAsync(model.Email, model.Password);
                if (user == null)
                {
                    return StatusCode(401, new { message = "Invalid email or password." });
                }

                var token = _jwtService.GenerateToken(user.Id, user.Email);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,   
                    SameSite = SameSiteMode.None, 
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);

                return Ok(new
                {
                    success = true,
                    userId = user.Id,
                    message = "You successfully logged in"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("AuthToken");
                return Ok(new { message = "Logged out" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}