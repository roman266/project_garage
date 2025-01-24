using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;

namespace project_garage.Controllers
{
    public class MessageController : Controller
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public MessageController(IConversationService conversationService, IMessageService messageService, IUserService userService)
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _userService = userService;
        }

        [HttpPost]
        [Route("Message/Send/{conversationId}")]
        public async Task<IActionResult> SendMessage(string text, string conversationId)
        {
            try
            {
                var user = await _userService.GetByIdAsync(User.GetUserId());
                await _messageService.SendMessageAsync(text, conversationId, user.Id, user.UserName);
                return Json(new { success = true, message = "Message successfully sended" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Message/Delete")]
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
            try
            {
                await _messageService.DeleteMessageAsync(messageId);
                return Json(new { success = true, message = "Message successfully deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Message/Read")]
        public async Task<IActionResult> ReadMessage(string messageId)
        {
            try
            {
                await _messageService.ReadMessageAsync(messageId);
                return Json(new { success = true, message = "Message successfully readen" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Message/DeleteForMe")]
        public async Task<IActionResult> DeleteForMe(string messageId)
        {
            try
            {
                await _messageService.DeleteMessageForMeAsync(messageId);
                return Json(new { success = true, message = "Message successfully readen" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
