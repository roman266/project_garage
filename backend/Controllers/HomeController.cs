using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Interfaces.IService;
using project_garage.Data;
using System;
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

        public HomeController(
            IPostService postService,
            IFriendService friendService,
            IUserService userService,
            IRecommendationService recommendationService)
        {
            _postService = postService;
            _friendService = friendService;
            _userService = userService;
            _recommendationService = recommendationService;
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetUserFeed(string? lastRequestId, int limit = 10)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                Console.WriteLine($"Request: userId={userId}, lastRequestId={lastRequestId}, limit={limit}");

                var response = await _recommendationService.GetUserFeedAsync(userId, lastRequestId, limit);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}