using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using project_garage.Models.Enums;

namespace project_garage.Service
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _repository;

        public ReactionService(IReactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CheckForExistanceAsync(string userId, string entityId, EntityType entityType)
        {
            return await _repository
                .GetByEntityAndUserIdAsync(userId, entityType, entityId) != null;
        }

        public async Task SendReactionAsync(string entityId, ReactionType reactionType, EntityType entityType, string userId)
        {
            if (await CheckForExistanceAsync(userId, entityId, entityType) == true)
            {
                throw new InvalidOperationException("Reaction between this user and entity already exist");
            }

            var reaction = new ReactionModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                ReactionType = reactionType,
                EntityId = entityId,
                EntityType = entityType,
            };

            await _repository.CreateNewAsync(reaction);
        }

        public async Task<List<ReactionModel>> GetEntityReactionsAsync(string entityId, string type)
        {
            if (!Enum.TryParse<EntityType>(type, out var entityType) || !Enum.IsDefined(typeof(EntityType), entityType))
            {
                throw new ArgumentException($"Invalid EntityType: {type}");
            }

            var reactionList = await _repository.GetByEntityIdAsync(entityId, entityType);

            return reactionList;
        }

        public async Task<int> GetUserReactionCountAsync(string userId)
        {
            var reactionList = await _repository.GetByUserIdAsync(userId);
            var reactionCount = reactionList.Count();

            return reactionCount;
        }

        public async Task DeleteReactionAsync(string reactionId)
        {
            var reaction = await _repository.GetByIdAsync(reactionId);

            if (reaction == null)
                throw new KeyNotFoundException($"Reaction with id: {reactionId} does not exist.");

            await _repository.DeleteAsync(reaction);
        }
    }
}
