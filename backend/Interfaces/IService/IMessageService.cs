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
        Task DeleteMessageAsync(string messageId);
        Task DeleteMessageForMeAsync(string messageId);
    }
}
