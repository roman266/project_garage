using Microsoft.AspNetCore.SignalR;

namespace project_garage.Data
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string conversationId, string senderId, string senderName, string message)
        {
            // Транслюємо всім у конкретній кімнаті (групі)
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                SenderName = senderName,
                Text = message,
                SendedAt = DateTime.UtcNow
            });
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }
    }
}
