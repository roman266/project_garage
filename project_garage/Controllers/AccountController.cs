using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

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
                var authDto = await _authService.SignInAsync(model.Email, model.Password);
                Response.Cookies.Append("AuthToken", authDto.JwtToken, authDto.CookieOptions);

                return Ok( new { message = "You successfully logged in" });
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