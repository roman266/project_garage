using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;
using project_garage.Models.ViewModels;
using project_garage.Service;

namespace project_garage.Controllers
{
    [Route("api/friends")]
    [Authorize]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpPost("send/{friendId}")]
        public async Task<IActionResult> SendRequest(string friendId)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _friendService.SendFriendRequestAsync(friendId, userId);
                return Ok(new { message = "Request sended" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("accept/{requestId}")]
        public async Task<IActionResult> AcceptFriend(string requestId)
        {
            try
            {
                await _friendService.AcceptRequestAsync(requestId);
                return Ok(new { message = "Request accepted"});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("reject/{requestId}")]
        public async Task<IActionResult> RejectFriend(string requestId)
        {
            try
            {
                await _friendService.RejectOrDeleteAsync(requestId);
                return Ok(new { message = "Request rejected" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("my-requests/outcoming")]
        public async Task<IActionResult> GetUnacceptedRequests(string? lastRequestId, int limit = 20)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var requests = await _friendService.GetOutcomingRequestsAsync(userId, lastRequestId, limit);
                return Ok(requests);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("my-requests/incoming")]
        public async Task<IActionResult> GetIncomingRequests(string? lastRequestId, int limit = 20)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var requests = await _friendService.GetIncomingRequestsAsync(userId, lastRequestId, limit);
                return Ok(requests);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("my-requests/friends")]
        public async Task<IActionResult> GetAcceptedRequests(string? lastfriendId, int limit = 20)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var requests = await _friendService.GetFriendsAsync(userId, lastfriendId, limit);
                return Ok(requests);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
