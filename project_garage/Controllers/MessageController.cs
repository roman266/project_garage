using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;

namespace project_garage.Controllers
{
    [Authorize]
    [Route("api/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IReactionService _reactionService;

        public MessageController(IConversationService conversationService, IMessageService messageService, IUserService userService, IReactionService reactionService)
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _userService = userService;
            _reactionService = reactionService;
        }

        [HttpPost("{conversationId}/send")]
        public async Task<IActionResult> SendMessage(string text, string conversationId)
        {
            try
            {
                var logedUserId = UserHelper.GetCurrentUserId(HttpContext);
                var user = await _userService.GetByIdAsync(logedUserId);
                await _messageService.SendMessageAsync(text, conversationId, user.Id, user.UserName);
                return Ok(new { message = "Message successfully sended" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete/{messageId}")]
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
            try
            {
                await _messageService.DeleteMessageAsync(messageId);
                return Ok(new { message = "Message successfully deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occured", details = ex.Message });
            }
        }

        [HttpPost("read/{messageId}")]
        public async Task<IActionResult> ReadMessage(string messageId)
        {
            try
            {
                await _messageService.ReadMessageAsync(messageId);
                return Ok(new { message = "Message successfully readen" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occured", details = ex.Message });
            }
        }

        [HttpPost("deleteForMe/{messageId}")]
        public async Task<IActionResult> DeleteForMe(string messageId)
        {
            try
            {
                await _messageService.DeleteMessageForMeAsync(messageId);
                return Ok(new { message = "Message successfully deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occured", details = ex.Message });
            }
        }

        [HttpGet("reactions/{messageId}")]
        public async Task<IActionResult> GetMessageReactions(string messageId)
        {
            try
            {
                var reactionList = await _reactionService.GetEntityReactionsAsync(messageId, "Message");
                return Ok(new { reactions = reactionList, message = "Success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occured", details = ex.Message });
            }
        }
    }
}
