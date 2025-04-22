using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using project_garage.Data;
using project_garage.Models.DTOs;

namespace project_garage.Controllers
{
    [Authorize]
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly IFriendService _friendService;
        private readonly IReactionService _reactionService;
        private readonly ICloudinaryService _cloudinaryService;

        public ProfileController(IUserService userRepository, IFriendService friendService, IPostService postService,
            IReactionService reactionService, ICloudinaryService cloudinaryService)
        {
            _userService = userRepository;
            _friendService = friendService;
            _postService = postService;
            _reactionService = reactionService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUserProfileInfo(string userId)
        {
            try
            {
                var loggedInUserId = UserHelper.GetCurrentUserId(HttpContext); // Отримуємо ID залогіненого користувача
                var user = await _userService.GetByIdAsync(userId);
                var canAddFriend = await _friendService.IsFriendAsync(loggedInUserId, userId);

                var viewModel = new ProfileDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePicture = user.ProfilePicture,
                    Description = user.Description,
                    FriendsCount = await _friendService.GetFriendsCount(userId),
                    PostsCount = _postService.GetCountOfPosts(userId),
                    ReactionsCount = await _reactionService.GetUserReactionCountAsync(userId),
                    CanAddFriend = !canAddFriend,
                    ActiveStatus = user.ActiveStatus
                };
                return Ok(new { profile = viewModel });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error ocurred", details = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserProfileInfo()
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var user = await _userService.GetByIdAsync(userId);
                var viewModel = new ProfileDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePicture = user.ProfilePicture,
                    Description = user.Description,
                    FriendsCount = await _friendService.GetFriendsCount(userId),
                    PostsCount = _postService.GetCountOfPosts(userId),
                    CanAddFriend = false,
                    ActiveStatus = user.ActiveStatus
                };
                return Ok(new { profile = viewModel });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error ocurred", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("me/edit")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileDto editProfileDto)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _userService.UpdateUserInfoAsync(userId, editProfileDto.UserName, editProfileDto.FirstName, 
                    editProfileDto.LastName, editProfileDto.Description, editProfileDto.Email, editProfileDto.Password);

                return Ok(new { message = "User info successfully updated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error ocurred", details = ex.Message });
            }
        }

        [HttpGet]
        [Route("search-users")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchBoxDto model, string? lastUserId, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(model.Query))
                return BadRequest(new { message = "Search query cannot be empty" });

            try
            {
                var users = await _userService.SearchUsersAsync(model.Query, lastUserId, limit);
                return Ok(new { message = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);

                var avatarUrl = await _cloudinaryService.UploadImageAsync(file);
                await _userService.UpdateProfilePictureAsync(userId, avatarUrl);
                return Ok(new { avatarUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Upload failed", error = ex.Message });
            }
        }

        [HttpPatch("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto model)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _userService.ChangeUserEmailAsync(model.Password, model.Email, userId);
                return Ok("Email changed successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpPost("send-password-verify-email")]
        public async Task<IActionResult> SendPasswordVereficationEmail()
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _userService.SendPasswordResetEmailAsync(userId);
                return Ok("Email sended successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _userService.ChangeUserPasswordAsync(userId, model.NewPassword, model.ConfirmationCode);
                return Ok("Password changed successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}