using project_garage.Models.DbModels;
using project_garage.Models.Enums;

namespace project_garage.Interfaces.IService
{
    public interface IReactionService
    {
        Task<bool> CheckForExistanceAsync(string userId, string entityId, EntityType entityType);
        Task SendReactionAsync(string entityId, string actionId, ReactionType reactionType, EntityType entityType, string userId);
        Task<List<ReactionModel>> GetEntityReactionsAsync(string entityId, string entityType);
        Task<int> GetUserReactionCountAsync(string userId);
        Task DeleteReactionAsync(string reactionId);
    }
}
