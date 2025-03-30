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
        private readonly ICloudinaryService _cloudinaryService;

        public MessageController(IMessageService messageService, IConversationService conversationService, ICloudinaryService cloudinaryService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationMessage(string conversationId, string? lastMessageId, int messageLimit = 20)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var messages = await _messageService.GetPaginatedConversationMessagesAsync(conversationId, lastMessageId, messageLimit, userId);
                return Ok(messages);
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

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            try
            {
                var url = await _cloudinaryService.UploadImageAsync(image);
                return Ok(url);
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
