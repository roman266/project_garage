using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using project_garage.Data;
using project_garage.Interfaces.IService;
using project_garage.Models.ViewModels;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public MessageController(
            IMessageService messageService, 
            IConversationService conversationService, 
            ICloudinaryService cloudinaryService)
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

        [HttpPatch("{conversationId}/{messageId}/read")]
        public async Task<IActionResult> ReadMessage(string conversationId, string messageId)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var isReaden = await _messageService.ReadMessageAsync(messageId, userId);
                return Ok(isReaden);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageOnCreationDto messageDto)
        {
            try
            {
                var senderId = UserHelper.GetCurrentUserId(HttpContext);
                var message = await _messageService.AddMessageAsync(messageDto, senderId);
                return Ok(message);
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

        [HttpDelete("{messageId}/delete")]
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
            try
            {
                await _messageService.DeleteMessageAsync(messageId);
                return Ok();
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

        [HttpPatch("{messageId}/update")]
        public async Task<IActionResult> EditMessage(string messageId, [FromBody] string editedText)
        {
            try
            {
                await _messageService.UpdateMessageTextAsync(messageId, editedText);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
