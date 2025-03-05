using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace project_garage.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly IFriendService _friendService;
        private readonly IUserService _userService;

        public HomeController(IPostService postService, IFriendService friendService, IUserService userService)
        {
            _postService = postService;
            _friendService = friendService;
            _userService = userService;
        }

        private IActionResult JsonResponse(object data, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            return Json(data);
        }

        [HttpGet]
        [Route("Home/Index")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return JsonResponse(new { success = false, message = "Unauthorized" }, 401);
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return JsonResponse(new { success = false, message = "User not found" }, 404);
            }

            var friends = await _friendService.GetByUserIdAsync(userId);
            var friendIds = friends.Where(f => f.IsAccepted).Select(f => f.FriendId).ToList();

            var posts = new List<PostModel>();
            foreach (var friendId in friendIds)
            {
                var friendPosts = await _postService.GetPostsByUserIdAsync(friendId);
                posts.AddRange(friendPosts);
            }

            var userPosts = await _postService.GetPostsByUserIdAsync(userId);
            posts.AddRange(userPosts);

            posts = posts.OrderByDescending(p => p.CreatedAt).ToList();

            var viewModel = new HomeViewModel
            {
                User = user,
                Posts = posts,
                FriendsCount = friends.Count
            };

            return JsonResponse(new { success = true, data = viewModel });
        }
    }
}
