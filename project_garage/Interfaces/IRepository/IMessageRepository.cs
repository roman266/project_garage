using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IMessageRepository
    {
        Task CreateNewAsync(MessageModel message);
        Task<MessageModel> GetByIdAsync(string id);
        Task<List<MessageModel>> GetByUserIdAsync(string id);
        Task<List<MessageModel>> GetByConversationId(string id);
        Task<List<MessageDto>> GetMessagesForUserByConversationIdAsync(string conversationId, string userId);
        Task UpdateAsync(MessageModel message);
        Task DeleteAsync(MessageModel message);
    }
}
