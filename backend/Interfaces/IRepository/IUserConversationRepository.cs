using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IUserConversationRepository
    {
        Task<bool> IsUserInConversationAsync(string userId, string conversationId);
        Task AddUserToConversationAsync(UserConversationModel userConversation);
        Task<bool> ExistsPrivateConversationAsync(string userId, string anotherUserId);
        Task RemoveUserFromConversationAsync(string userId, string conversationId);
        Task<List<ConversationModel>> GetPaginatedUserConversationsAsync(string userId, string? lastConversationId, int limit);
    }
}
