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

        private IActionResult JsonResponse(object data, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            return Json(data);
        }

        [HttpPost]
        [Route("Conversation/Start")]
        public async Task<IActionResult> StartConversation(string secondUserId)
        {
            try
            {
                //if users don't exist we get exception
                var user1 = await _userService.GetByIdAsync(User.GetUserId());
                var user2 = await _userService.GetByIdAsync(secondUserId);

                await _conversationService.CreateConversationAsync(User.GetUserId(), secondUserId);

                return JsonResponse(new { success = true, message = "New conversation successfully started" });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpPost]
        [Route("Conversation/Delete")]
        public async Task<IActionResult> DeleteConversation(string conversationId)
        {
            try
            {
                await _conversationService.DeleteConversationAsync(conversationId);

                return JsonResponse(new { success = true, message = "Conversation successfully deleted" });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpGet]
        [Route("Conversation/GetConversationMessages/{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(string conversationId)
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
                        WriteIndented = true
                    };
                    Response.StatusCode = 200;
                    return Json(new { success = true, currentUserMessagesLst = currentUserMessages, secondUserMessagesLst = secondUserMessages }, jsonOptions);
                }
                else if (conversation.User2Id == User.GetUserId())
                {
                    var secondUserMessages = await _conversationService.GetMessagesForUserByConversationIdAsync(conversationId, conversation.User1Id);
                    var jsonOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                        WriteIndented = true
                    };
                    Response.StatusCode = 200;
                    return Json(new { success = true, currentUserMessagesLst = currentUserMessages, secondUserMessagesLst = secondUserMessages }, jsonOptions);
                }

                return JsonResponse(new { success = false, message = "Something went wrong" }, 500);
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpGet]
        [Route("Conversation/GetCurrUserConversations")]
        public async Task<IActionResult> GetCurrUserConversations()
        {
            try
            {
                var conversations = await _conversationService.GetConversationByUserIdAsync(User.GetUserId());
                return JsonResponse(new { success = true, message = "Conversation successfully deleted", conversationList = conversations });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }
    }
}
