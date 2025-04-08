using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using project_garage.Models.DTOs;

namespace project_garage.Data
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinChat(string conversationId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task SendMessage(SendMessageDto message)
        {
            try
            {
                await Clients.OthersInGroup(message.ConversationId).SendAsync("ReceiveMessage",
                    message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task ReadMessage(string conversationId, string messageId)
        {
            try
            {
                await Clients.Group(conversationId).SendAsync("MessageReaden", messageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task DeleteMessage(string messageId, string conversationId)
        {
            try
            {
                await Clients.Group(conversationId).SendAsync("MessageDeleted", messageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task UpdateMessage(string messageId, string conversationId, string editedText)
        {
            try
            {
                await Clients.Group(conversationId).SendAsync("MessageEdited", messageId, editedText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task LeaveChat(string conversationId)
        {
            var userId = Context.User?.FindFirst("sub")?.Value;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
