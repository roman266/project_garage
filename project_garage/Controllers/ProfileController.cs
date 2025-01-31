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

        [Route("Profile/ProfileIndex/{userId}")]
        public async Task<IActionResult> ProfileIndex(string userId)
        {
            var loggedInUserId = User.GetUserId(); // Отримуємо ID залогіненого користувача

            Console.WriteLine($"USER ID: {userId}, LOGGED IN USER ID: {loggedInUserId}");
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var canAddFriend = await _friendService.CanAddFriendAsync(loggedInUserId, userId);

            Console.WriteLine($"USER ID: {userId}");

            var viewModel = new ProfileViewModel
            {
                UserId = user.Id,
                Nickname = user.UserName,
                Description = user.Description,
                FriendsCount = await _friendService.GetFriendsCount(userId),
                PostsCount = await _postService.GetCountOfPosts(userId),
                CanAddFriend = canAddFriend
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("Profile/SearchUsers")]
        public async Task<IActionResult> SearchUsers(SearchBoxViewModel model)
        {

            if (!string.IsNullOrEmpty(model.Query))
            {
                var users = await _userService.SearchUsersAsync(model.Query);
                return PartialView("_UserList", users);
            }

            return PartialView("_UserList", new List<UserModel>());
        }
    }
}
