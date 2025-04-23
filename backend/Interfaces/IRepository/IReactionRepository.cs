using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IReactionRepository
    {
        Task CreateNewAsync(ReactionModel model);
        Task<ReactionModel> GetByIdAsync(string id);
        Task<ReactionTypeModel> GetReactionTypeByIdAsync(string reactionTypeId);
        Task UpdateAsync(ReactionModel model);
        Task<List<ReactionModel>> GetByUserIdAsync(string id);
        Task<List<ReactionModel>> GetByEntityIdAsync(string entityId); 
        Task<ReactionModel> GetByEntityAndUserIdAsync(string userId, string entityId); 
        Task DeleteAsync(ReactionModel action);
    }
}