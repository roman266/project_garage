using Microsoft.AspNetCore.SignalR;
using project_garage.Interfaces.IService;
using project_garage.Models.ViewModels;

namespace project_garage.Data
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly IUserConversationService _userConversationService;

        public ChatHub(IMessageService messageService, IConversationService conversationService, IUserConversationService userConversationService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _userConversationService = userConversationService;
        }

        // Підключення користувача до чату (групи)
        public async Task JoinChat(string conversationId)
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            await _userConversationService.AddUserToConversationAsync(userId, conversationId);

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            await Clients.Group(conversationId).SendAsync("ReceiveSystemMessage", $"User {Context.ConnectionId} joined the chat.");
        }

        // Відправка повідомлення в конкретну групу
        public async Task SendMessage(MessageOnCreationDto messageDto)
        {
            await _messageService.AddMessageAsync(messageDto);
            await Clients.Group(messageDto.ConversationId).SendAsync("ReceiveMessage", messageDto.SenderId, messageDto.Text);
        }

        // Відключення користувача від чату (групи)
        public async Task LeaveChat(string conversationId)
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            await _userConversationService.RemoveUserFromConversationAsync(userId, conversationId);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
            await Clients.Group(conversationId).SendAsync("ReceiveSystemMessage", $"User {Context.ConnectionId} left the chat.");
        }

        // Обробка відключення користувача
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Тут можна видаляти користувача з груп, якщо потрібно
            await base.OnDisconnectedAsync(exception);
        }
    }

}
