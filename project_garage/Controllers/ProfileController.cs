using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using project_garage.Models.DbModels;
using project_garage.Data;

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
        [Route("Profile/ProfileIndex/{userId}")]
        public async Task<IActionResult> ProfileIndex(string userId)
        {
            try
            {
                var loggedInUserId = User.GetUserId();

                Console.WriteLine($"USER ID: {userId}, LOGGED IN USER ID: {loggedInUserId}");
                var user = await _userService.GetByIdAsync(userId);


                var canAddFriend = true;

                if (await _friendService.IsFriendAsync(loggedInUserId, userId))
                    canAddFriend = false;

                var viewModel = new ProfileViewModel
                {
                    UserId = user.Id,
                    Nickname = user.UserName,
                    Description = user.Description,
                    FriendsCount = await _friendService.GetFriendsCount(userId),
                    PostsCount = await _postService.GetCountOfPosts(userId),
                    CanAddFriend = canAddFriend
                };
                return Json(new { success = true, message = $"Profile {userId}", info = viewModel });
            }
            catch (InvalidDataException ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, error = ex.Message }, 400);
            }


        }

        [HttpGet]
        [Route("Profile/SearchUser/")]
        public async Task<IActionResult> SearchUsers(SearchBoxViewModel query)
        {
            try
            {
                var users = await _userService.SearchUsersAsync(query.Query);
                return Json(new { success = true, message = "Users successfully found", usersLst = users });

            }
            catch (InvalidDataException ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
            catch (InvalidOperationException ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 400);
            }
        }


    }
}
