using project_garage.Models.DbModels;
using project_garage.Models.DTOs;

namespace project_garage.Interfaces.IRepository
{
    public interface IUserConversationRepository
    {
        Task<bool> IsUserInConversationAsync(string userId, string conversationId);
        Task AddUserToConversationAsync(UserConversationModel userConversation);
        Task<bool> ExistsPrivateConversationAsync(string userId, string anotherUserId);
        Task<List<UserModel>> GetConversationMembersAsync(string conversationId);
        Task RemoveUserFromConversationAsync(string userId, string conversationId);
        Task<List<ConversationDisplayDto>> GetPaginatedUserConversationsAsync(string userId, string? lastConversationId, int limit);
        Task<string> GetPrivateConversationBetweenUsersAsync(string userId, string friendId);
    }
}
