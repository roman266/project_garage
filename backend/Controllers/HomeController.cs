using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IPostService postService,
            IFriendService friendService,
            IUserService userService,
            IRecommendationService recommendationService,
            ILogger<HomeController> logger)
        {
            _postService = postService;
            _friendService = friendService;
            _userService = userService;
            _recommendationService = recommendationService;
            _logger = logger;
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetUserFeed(int page = 1, int limit = 10)
        {
            try
            { 
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var friends = await _friendService.GetByUserIdAsync(userId);
                var friendIds = friends.Where(f => f.IsAccepted).Select(f => f.FriendId).ToList();

                var posts = new List<PostModel>();
                foreach (var friendId in friendIds)
                {
                    var friendPosts = await _postService.GetPaginatedPostsByUserIdAsync(friendId, null, 10);
                    posts.AddRange(await _postService.GetPostsByUserIdAsync(friendId));
                }
                posts.AddRange(await _postService.GetPostsByUserIdAsync(userId));

                var userPosts = await _postService.GetPaginatedPostsByUserIdAsync(userId, null, 10);
                posts = posts.OrderByDescending(p => p.CreatedAt).ToList();
                var recommendedPosts = await _recommendationService.GetRecommendedPostsAsync(userId);
                var orderedPosts = posts.Concat(recommendedPosts).OrderByDescending(p => p.CreatedAt).ToList();

                int totalPages = (int)Math.Ceiling((double)orderedPosts.Count / limit);

                var paginatedPosts = orderedPosts
                    .Skip((page - 1) * limit) 
                    .Take(limit) 
                    .Select(p => new
                    {
                        p.Id,
                        p.Content,
                        p.CreatedAt,
                        Author = new
                        {
                            p.User?.Id,
                            p.User?.FirstName,
                            p.User?.LastName,
                            p.User?.UserName,
                            p.User?.ProfilePicture
                        }
                    }).ToList();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        posts = paginatedPosts,
                        totalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}