using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IConversationService
    {
        Task CreateConversationAsync(string user1Id, string user2Id);
        Task<List<ConversationModel>> GetConversationByUserIdAsync(string userId);
        Task<ConversationModel> GetConversationByIdAsync(string id);
        Task<List<MessageModel>> GetMessagesForUserByConversationIdAsync(string conversationId, string userId);
        Task DeleteConversationAsync(string conversationId);
    }
}
