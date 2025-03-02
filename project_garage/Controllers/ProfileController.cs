using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using project_garage.Data;

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

        public ProfileController(IUserService userRepository, IFriendService friendService, IPostService postService, IReactionService reactionService)
        {
            _userService = userRepository;
            _friendService = friendService;
            _postService = postService;
            _reactionService = reactionService;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUserProfileInfo(string userId)
        {
            try
            {
                var loggedInUserId = UserHelper.GetCurrentUserId(HttpContext); // Отримуємо ID залогіненого користувача
                var user = await _userService.GetByIdAsync(userId);
                var canAddFriend = await _friendService.CanAddFriendAsync(loggedInUserId, userId);

                var viewModel = new ProfileDto
                {
                    UserId = user.Id,
                    Nickname = user.UserName,
                    Description = user.Description,
                    FriendsCount = await _friendService.GetFriendsCount(userId),
                    PostsCount = await _postService.GetCountOfPosts(userId),
                    ReactionsCount = await _reactionService.GetUserReactionCountAsync(userId),
                    CanAddFriend = canAddFriend
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
                    Nickname = user.UserName,
                    Description = user.Description,
                    FriendsCount = await _friendService.GetFriendsCount(userId),
                    PostsCount = await _postService.GetCountOfPosts(userId),
                    CanAddFriend = false
                };
                return Ok(new { profile = viewModel });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error ocurred", delatils = ex.Message });
            }
        }

        [HttpGet]
        [Route("searchUsers")]
        public async Task<IActionResult> SearchUsers(SearchBoxViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Query))
            {
                return BadRequest(new { message = "Search query cannot be empty" });
            }

            try
            {
                var users = await _userService.SearchUsersAsync(model.Query);
                return Ok(new { message = users });
                
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
