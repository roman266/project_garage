using project_garage.Models.DbModels;
using project_garage.Models.DTOs;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface IMessageService
    {
        Task<SendMessageDto> AddMessageAsync(MessageOnCreationDto messageOnCreationDto, string senderId);
        Task<bool> ReadMessageAsync(string messageId, string senderId);
        Task<List<MessageDto>> GetPaginatedConversationMessagesAsync(string conversationId, string lastMessageId, int messageCountLimit, string userId);
        Task<List<MessageModel>> GetMessagesByUserIdAsync(string userId);
        Task<int> GetConversationUnreadedMessagesCountAsync(string conversationId, string userId);
        Task<int> GetUserUnreadedMessagesCountAsync(string userId);
        Task UpdateMessageTextAsync(string messageId, string editedText);
        Task DeleteMessageAsync(string messageId);
        Task DeleteMessageForMeAsync(string messageId);
    }
}
