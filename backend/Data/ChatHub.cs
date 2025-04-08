using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using project_garage.Models.DTOs;
using project_garage.Models.ViewModels;
using System.Security.Claims;

namespace project_garage.Data
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinChat(string conversationId)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new HubException("Unauthorized");
                }

                Console.WriteLine(userId);

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
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    throw new HubException("Unauthorized");

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
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    throw new HubException("Unauthorized");

                await Clients.Group(conversationId).SendAsync("MessageReaden", messageId);
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
