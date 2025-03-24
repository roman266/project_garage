using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Models.Enums;

namespace project_garage.Repository
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly ApplicationDbContext _context;

        public ReactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNewAsync(ReactionModel model)
        {
            _context.ReactionActions.Add(model);
            await _context.SaveChangesAsync();
        } 

        public async Task<ReactionModel> GetByIdAsync(string id)
        {
            var reaction = await _context
                .ReactionActions
                .FirstOrDefaultAsync(ra => ra.Id == id);

            return reaction;
        }

        public async Task UpdateAsync(ReactionModel model)
        {
            _context.ReactionActions.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReactionModel>> GetByUserIdAsync(string id)
        {
            var list = await _context.ReactionActions.Where(ra => ra.UserId == id)
                .ToListAsync();

            return list;
        }

        public async Task<List<ReactionModel>> GetByEntityIdAsync(string id, EntityType entityType)
        {
            var list = await _context.ReactionActions
                .Where(ra => ra.EntityId == id && ra.EntityType == entityType)
                .ToListAsync();

            return list;
        }

        public async Task<ReactionModel> GetByEntityAndUserIdAsync(string userId, EntityType entityType, string entityId)
        {
            var reaction = await _context.ReactionActions
                .FirstOrDefaultAsync(ra => ra.UserId == userId && ra.EntityType == entityType && ra.EntityId == entityId);

            return reaction ?? new ReactionModel();
        }

        public async Task DeleteAsync(ReactionModel action)
        {
            _context.ReactionActions.Remove(action);
            await _context.SaveChangesAsync();
        }
    }
}
