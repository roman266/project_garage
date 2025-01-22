using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;
using System.Text.Json;

namespace project_garage.Controllers
{
    [Authorize]
    public class ConversationController : Controller
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
        [Route("Conversation/Start")]
        public async Task<IActionResult> StartConversation(string user2Id)
        {
            Console.WriteLine("in\'m here");
            Console.WriteLine(user2Id);
            try
            {
                //if users don't exist we get exception
                var user1 = await _userService.GetByIdAsync(User.GetUserId());
                var user2 = await _userService.GetByIdAsync(user2Id);

                await _conversationService.CreateConversationAsync(User.GetUserId(), user2Id);

                return Json(new { success = true, message = "New conversation successfully started" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Conversation/Delete")]
        public async Task<IActionResult> DeleteConversation(string conversationId)
        {
            try
            {
                await _conversationService.DeleteConversationAsync(conversationId);
                return Json(new { success = true, message = "Conversation successfully deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.Message}" });
            }
        }

        [HttpPost]
        [Route("Conversation/User/{conversationId}")]
        public async Task<IActionResult> ShowConversationMessages(string conversationId)
        {
            try
            {
                var conversation = await _conversationService.GetConversationByIdAsync(conversationId);

                var currentUserMessages = await _conversationService.GetMessagesForUserByConversationIdAsync(conversationId, User.GetUserId());

                if (conversation.User1Id == User.GetUserId())
                {
                    var secondUserMessages = await _conversationService.GetMessagesForUserByConversationIdAsync(conversationId, conversation.User2Id);
                    var jsonOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                        WriteIndented = true // Зручно для перегляду
                    };
                    return Json(new { success = true, currentUserMessagesLst = currentUserMessages, secondUserMessagesLst = secondUserMessages }, jsonOptions);
                }
                else if (conversation.User2Id == User.GetUserId())
                {
                    var secondUserMessages = await _conversationService.GetMessagesForUserByConversationIdAsync(conversationId, conversation.User1Id);
                    var jsonOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                        WriteIndented = true // Зручно для перегляду
                    };
                    return Json(new { success = true, currentUserMessagesLst = currentUserMessages, secondUserMessagesLst = secondUserMessages }, jsonOptions);
                }

                return Json(new { success = false, message = "Something went wrong" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.Message}" });
            }
        }


        [HttpPost]
        [Route("Conversation/User")]
        public async Task<IActionResult> ShowUserConversation()
        {
            try
            {
                var conversations = await _conversationService.GetConversationByUserIdAsync(User.GetUserId());
                return Json(new { success = true, message = "Conversation successfully deleted", conversationList = conversations });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{ex.Message}" });
            }
        }
    }
}
