using project_garage.Models.DbModels;
using project_garage.Models.Enums;

namespace project_garage.Interfaces.IRepository
{
    public interface IReactionRepository
    {
        Task CreateNewAsync(ReactionModel model);
        Task<ReactionModel> GetByIdAsync(string id);
        Task<List<ReactionModel>> GetByUserIdAsync(string userId);
        Task<List<ReactionModel>> GetByEntityIdAsync(string entityId, EntityType entityType);
        Task UpdateAsync(ReactionModel model);
        Task<ReactionModel> GetByEntityAndUserIdAsync(string userId, EntityType entityType, string entityId);
        Task DeleteAsync(ReactionModel model);
     
    }
}
