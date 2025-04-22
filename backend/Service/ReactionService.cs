using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;

namespace project_garage.Service
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _repository;

        public ReactionService(IReactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CheckForExistanceAsync(string userId, string entityId)
        {
            return await _repository.GetByEntityAndUserIdAsync(userId, entityId) != null;
        }

        public async Task SendReactionAsync(string entityId, string reactionTypeId, string userId)
        {
            if (await CheckForExistanceAsync(userId, entityId))
            {
                throw new InvalidOperationException("Reaction between this user and entity already exists");
            }

            // Перевірка, чи існує ReactionTypeId
            var reactionType = await _repository.GetReactionTypeByIdAsync(reactionTypeId);
            if (reactionType == null)
            {
                throw new ArgumentException($"Invalid ReactionTypeId: {reactionTypeId}");
            }

            var reaction = new ReactionModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                ReactionTypeId = reactionTypeId,
                EntityId = entityId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateNewAsync(reaction);
        }

        public async Task<List<ReactionModel>> GetEntityReactionsAsync(string entityId)
        {
            var reactionList = await _repository.GetByEntityIdAsync(entityId);
            return reactionList;
        }

        public async Task<int> GetUserReactionCountAsync(string userId)
        {
            var reactionList = await _repository.GetByUserIdAsync(userId);
            return reactionList.Count;
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