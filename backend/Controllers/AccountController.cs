using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using project_garage.Models.DbModels;
using Microsoft.IdentityModel.Tokens;
using project_garage.Data;
using System.Net;
using System.Net.Http;

namespace project_garage.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AccountController(IUserService userService, IAuthService authService, ITokenService tokenService)
        {
            _userService = userService;
            _authService = authService;
            _tokenService = tokenService;
        }

        public void ClearAuthCookie()
        {
            Response.Cookies.Append("AccessToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            Response.Cookies.Append("RefreshToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto model)
        {
            try
            {
                var result = await _userService.CreateUserAsync(model.UserName, model.Email, model.Password);
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
                ClearAuthCookie();
                Response.Cookies.Append("AccessToken", authDto.AccessToken, authDto.AccessCookieOptions);
                Response.Cookies.Append("RefreshToken", authDto.RefreshToken, authDto.RefreshCookieOptions);
                return Ok( new { message = "You successfully logged in"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {

            try
            {
                var authDto = await _tokenService.RenewAccessTokenAsync();
                Response.Cookies.Append("AccessToken", authDto.AccessToken, authDto.AccessCookieOptions);
                Response.Cookies.Append("RefreshToken", authDto.RefreshToken, authDto.RefreshCookieOptions);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = HttpContext.Request.Cookies["RefreshToken"];

                if (!string.IsNullOrEmpty(refreshToken))
                    await _tokenService.RevokeRefreshTokenAsync(refreshToken);
                
                ClearAuthCookie();
                return Ok(new { message = "Logged out" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}