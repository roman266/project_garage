using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IConversationService
    {
        Task<ConversationModel> AddConversationAsync(bool isPrivate);
        Task<ConversationModel> GetConversationByIdAsync(string conversationId);
        Task CreatePrivateConversationBetweenUsersAsync(string user1Id, string user2Id);
        Task<List<ConversationModel>> GetPaginatedConversationsByUserIdAsync(string userId, string? lastConversationId, int limit);
        Task<bool> IsUserInConversationAsync(string userId, string conversationId);
        Task<bool> CheckConversationExistance(string conversationId);
        Task DeleteConversationAsync(string conversationId);
    }
}
