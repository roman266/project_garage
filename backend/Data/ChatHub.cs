using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using project_garage.Models.DbModels;
using project_garage.Models.DTOs;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IRepository;
using System.Security.Claims;

namespace project_garage.Data
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;

        public ChatHub(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public string GetUserId()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                throw new UnauthorizedAccessException();

            return userId;
        }

        public async Task OnLoginConnection()
        {
            try
            {
                var userId = GetUserId();

                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                
                // Оновлюємо статус користувача в базі даних
                await _userRepository.UpdateUserStatusAsync(userId, "Online");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task NotifyUsersAboutReceivedMessage(string conversationId, List<string> userIds, ProfileDto senderInfo)
        {
            try
            {
                foreach (var userId in userIds)
                {
                    if (userId != GetUserId())
                    {
                        await Clients.Group($"user_{userId}").SendAsync("ReceivedMessage", conversationId, senderInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task LogOut()
        {
            try
            {
                var userId = GetUserId();

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
                
                // Оновлюємо статус користувача в базі даних
                await _userRepository.UpdateUserStatusAsync(userId, "Offline");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

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
                await Clients.OthersInGroup(message.ConversationId).SendAsync("ReceiveMessage", message);
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
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var userId = GetUserId();

                // Оновлюємо статус користувача при відключенні
                await _userRepository.UpdateUserStatusAsync(userId, "Offline");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}