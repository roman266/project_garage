using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;
using System.Text.Json;

namespace project_garage.Controllers
{
    [Authorize]
    [Route("api/conversation")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public ConversationController(IConversationService conversationService, IMessageService messageService, IUserService userService)
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _userService = userService;
        }

        [HttpPost]
        [Route("start")]
        public async Task<IActionResult> StartConversation(string secondUserId)
        {
            try
            {
                var logedUserId = UserHelper.GetCurrentUserId(HttpContext);
                //if users don't exist we get exception
                var user1 = await _userService.GetByIdAsync(logedUserId);
                var user2 = await _userService.GetByIdAsync(secondUserId);

                await _conversationService.CreateConversationAsync(logedUserId, secondUserId);

                return Ok(new { message = "New conversation successfully started" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteConversation(string conversationId)
        {
            try
            {
                await _conversationService.DeleteConversationAsync(conversationId);

                return Ok(new { message = "Conversation successfully deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("get-messages/{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(string conversationId)
        {
            try
            {
                var logedUserId = UserHelper.GetCurrentUserId(HttpContext);

                var messages = await _messageService.GetUserMessagesByConversationIdAsync(conversationId, logedUserId);

                return Ok(messages);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("my-conversations")]
        public async Task<IActionResult> GetCurrUserConversations()
        {
            try
            {
                var logedUserId = UserHelper.GetCurrentUserId(HttpContext);
                var conversations = await _conversationService.GetConversationByUserIdAsync(logedUserId);
                return Ok(new { conversationList = conversations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
