using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;

namespace project_garage.Controllers
{
    [Route("api/message")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly IUserConversationService _userConversationService;

        public MessageController(IMessageService messageService, IConversationService conversationService, IUserConversationService userConversationService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _userConversationService = userConversationService;
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationMessage(string conversationId, string? lastMessageId, int messageLimit = 20)
        {
            try
            {
                if (await _conversationService.CheckConversationExistance(conversationId))
                {
                    var userId = UserHelper.GetCurrentUserId(HttpContext);

                    if (await _userConversationService.IsUserInConversationAsync(userId, conversationId))
                    {
                        var messages = _messageService.GetPaginatedConversationMessagesAsync(conversationId, lastMessageId, messageLimit);
                        return Ok(messages);
                    }

                    return StatusCode(500, "You don't have access to this conversation");
                }
                return StatusCode(500, "Conversation does not exist");
            }
            catch(ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
