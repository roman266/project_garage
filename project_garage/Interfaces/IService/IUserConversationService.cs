using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IUserConversationService
    {
        Task<UserConversationModel> AddUserToConversationAsync(string userId, string conversationId);
        Task<bool> IsUserInConversationAsync(string userId, string conversationId);
        Task<List<ConversationModel>> GetUserConversationsAsync(string userId);
        Task RemoveUserFromConversationAsync(string userId, string conversationId);
    }
}
