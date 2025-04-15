using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IMessageRepository
    {
        Task<MessageModel> CreateNewAsync(MessageOnCreationDto messageOnCreationDto, string senderId);
        Task<MessageModel> GetByIdAsync(string id);
        Task<List<MessageModel>> GetByUserIdAsync(string id);
        Task<List<MessageDto>> GetPaginatedMessagesByConversationId(string userId, string conversationId, string lastMessageId, int messageCountLimit);
        Task<List<MessageModel>> GetUnreadedMessagesByConversationIdAsync(string conversationId);
        Task<List<MessageModel>> GetUnreadedMessagesByUserIdAsync(string userId);
        Task UpdateAsync(MessageModel message);
        Task DeleteAsync(MessageModel message);
    }
}
