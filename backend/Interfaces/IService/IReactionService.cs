using project_garage.Models.DbModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace project_garage.Interfaces.IService
{
    public interface IReactionService
    {
        Task<bool> CheckForExistanceAsync(string userId, string entityId); 
        Task SendReactionAsync(string entityId, string reactionTypeId, string userId); 
        Task<List<ReactionModel>> GetEntityReactionsAsync(string entityId); 
        Task<int> GetUserReactionCountAsync(string userId);
        Task DeleteReactionAsync(string reactionId);
    }
}