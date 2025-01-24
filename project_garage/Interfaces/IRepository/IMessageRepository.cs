using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IMessageRepository
    {
        Task CreateNewAsync(MessageModel message);
        Task<MessageModel> GetByIdAsync(string id);
        Task<List<MessageModel>> GetByUserIdAsync(string id);
        Task<List<MessageModel>> GetByConversationId(string id);
        Task UpdateAsync(MessageModel message);
        Task DeleteAsync(MessageModel message);
    }
}
