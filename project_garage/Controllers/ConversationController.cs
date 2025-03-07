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
        private readonly IUserConversationService _userConversationService;

        public ConversationController(IConversationService conversationService, IUserConversationService userConversationService) 
        {
            _conversationService = conversationService;
            _userConversationService = userConversationService;
        }

        [HttpPost("start/{recipientId}")]
        public async Task<IActionResult> StartConversationBetweenTwoUsers(string recipientId)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);

                

                /* if (!await _userConversationService.IsUserInConversationAsync(userId, conversation.Id)
                    && !await _userConversationService.IsUserInConversationAsync(recipientId, conversation.Id))
                    return StatusCode(500, new { message = "Conversation between this users already exist" }); */
                
                var conversation = await _conversationService.AddConversationAsync(true);

                await _userConversationService.AddUserToConversationAsync(userId, conversation.Id);
                await _userConversationService.AddUserToConversationAsync(recipientId, conversation.Id);

                return Ok(new { message = "Conversation started successfully" });
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
    }
}
