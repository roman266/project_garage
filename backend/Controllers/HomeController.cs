using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using project_garage.Data;

namespace project_garage.Controllers
{
    [Route("api/home")]
    [Authorize]
    [ApiController]
    public class HomeController : ControllerBase
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

        [HttpGet("feed")]
        public async Task<IActionResult> GetUserFeed(string? lastRequestId, int limit = 10)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                var friends = await _friendService.GetFriendsAsync(userId, lastRequestId, limit);

                var posts = new List<PostModel>();
                foreach (var friend in friends)
                {
<<<<<<< HEAD
                    var friendPosts = await _postService.GetPostsByUserIdAsync(friend.FriendId);
                    posts.AddRange(friendPosts);
=======
                    var friendPosts = await _postService.GetPaginatedPostsByUserIdAsync(friendId, null, 10);
>>>>>>> b1c80e9c34457a4010dee6a86f34a94b4096ae45
                }

                var userPosts = await _postService.GetPaginatedPostsByUserIdAsync(userId, null, 10);
                posts = posts.OrderByDescending(p => p.CreatedAt).ToList();

                var response = new
                {
                    success = true,
                    data = new
                    {
                        user,
                        posts,
                        friendsCount = friends.Count
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}