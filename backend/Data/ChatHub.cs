using Microsoft.AspNetCore.SignalR;
using project_garage.Interfaces.IService;
using project_garage.Models.ViewModels;
using System.Security.Claims;

namespace project_garage.Data
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task JoinChat(string conversationId)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new HubException("Unauthorized");
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task SendMessage(string conversationId, string text)
        {
            try
            {
                var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var message = new MessageOnCreationDto
                {
                    ConversationId = conversationId,
                    SenderId = senderId,
                    Text = text,
                };

                await _messageService.AddMessageAsync(message);
                
                await Clients.OthersInGroup(conversationId).SendAsync("ReceiveMessage", senderId, text);
            }
            catch (Exception ex) {
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
