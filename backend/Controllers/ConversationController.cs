using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;

namespace project_garage.Controllers
{
    [Route("api/conversations")]
    [Authorize]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost("start/{recipientId}")]
        public async Task<IActionResult> StartConversationBetweenTwoUsers(string recipientId)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var conversationId = await _conversationService.CreatePrivateConversationBetweenUsersAsync(userId, recipientId);

                return Ok(new { id = conversationId });
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

        [HttpPatch("message-sended/{conversationId}")]
        public async Task<IActionResult> UpdateLastSendedMessageTime(string conversationId)
        {
            try
            {
                await _conversationService.UpdateLastMessageAsync(conversationId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{conversationId}/get-members")]
        public async Task<IActionResult> GetConversationMembersIds(string conversationId)
        {
            try
            {
                var members = await _conversationService.GetConversationMembersIdsAsync(conversationId);
                return Ok(members);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occured");
            }
        }

        [HttpGet("my-conversations")]
        public async Task<IActionResult> GetCurrentUserConversations(string? lastConversationId, int limit = 15)
        {
            try
            {
                string userId = UserHelper.GetCurrentUserId(HttpContext);
                var conversations = await _conversationService.GetPaginatedConversationsByUserIdAsync(userId, lastConversationId, limit);

                return Ok(conversations);
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

        [HttpGet("get-private/{friendId}")]
        public async Task<IActionResult> GetPrivateConversationByFriendId(string friendId)
        {
            var userId = UserHelper.GetCurrentUserId(HttpContext);
            var conversationId = await _conversationService.GetPrivateConversationIdByFriendIdAsync(userId, friendId);

            if(conversationId == null)
                return NotFound();

            return Ok(new { id = conversationId });
        }
    }
}
