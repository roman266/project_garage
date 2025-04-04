using project_garage.Models.DbModels;
using project_garage.Models.DTOs;

namespace project_garage.Interfaces.IService
{
    public interface IConversationService
    {
        Task<ConversationModel> AddConversationAsync(bool isPrivate);
        Task UpdateLastMessageAsync(string conversationId);
        Task<ConversationModel> GetConversationByIdAsync(string conversationId);
        Task CreatePrivateConversationBetweenUsersAsync(string user1Id, string user2Id);
        Task<List<ConversationDisplayDto>> GetPaginatedConversationsByUserIdAsync(string userId, string? lastConversationId, int limit);
        Task<bool> IsUserInConversationAsync(string userId, string conversationId);
        Task<bool> CheckConversationExistance(string conversationId);
        Task DeleteConversationAsync(string conversationId);
    }
}
