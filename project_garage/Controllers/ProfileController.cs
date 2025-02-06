using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using project_garage.Models.DbModels;
using project_garage.Data;
using System.Diagnostics.Contracts;

namespace project_garage.Controllers
{

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly IFriendService _friendService;

        public ProfileController(IUserService userRepository, IFriendService friendService, IPostService postService)
        {
            _userService = userRepository;
            _friendService = friendService;
            _postService = postService;
        }

        private IActionResult JsonResponse(object data, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            return Json(data);
        }

        [HttpGet]
        [Route("Profile/GetUserProfileInfo/{userId}")]
        public async Task<IActionResult> GetUserProfileInfo(string userId)
        {
            try
            {
                var loggedInUserId = User.GetUserId(); // Отримуємо ID залогіненого користувача
                var user = await _userService.GetByIdAsync(userId);
                var canAddFriend = await _friendService.CanAddFriendAsync(loggedInUserId, userId);

                var viewModel = new ProfileViewModel
                {
                    UserId = user.Id,
                    Nickname = user.UserName,
                    Description = user.Description,
                    FriendsCount = await _friendService.GetFriendsCount(userId),
                    PostsCount = await _postService.GetCountOfPosts(userId),
                    CanAddFriend = canAddFriend
                };
                return JsonResponse(new { success = true, message = viewModel });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpGet]
        [Route("Profile/GetCurrentUserProfileInfo")]
        public async Task<IActionResult> GetCurrentUserProfileInfo()
        {
            try
            {
                var user = await _userService.GetByIdAsync(User.GetUserId());
                var viewModel = new ProfileViewModel
                {
                    UserId = user.Id,
                    Nickname = user.UserName,
                    Description = user.Description,
                    FriendsCount = await _friendService.GetFriendsCount(user.Id),
                    PostsCount = await _postService.GetCountOfPosts(user.Id),
                    CanAddFriend = false
                };
                return JsonResponse(new { success = true, message = viewModel });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }
        

        [HttpGet]
        [Route("Profile/SearchUsers")]
        public async Task<IActionResult> SearchUsers(SearchBoxViewModel model)
        {
            try
            {
                var users = await _userService.SearchUsersAsync(model.Query);
                return JsonResponse(new { success = true, list = users });
                
            }
            catch(Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }
    }
}
