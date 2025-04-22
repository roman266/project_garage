using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

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
            return await _context.ReactionActions
                .Include(r => r.ReactionType)
                .Include(r => r.User)
                .FirstOrDefaultAsync(ra => ra.Id == id);
        }

        public async Task<ReactionTypeModel> GetReactionTypeByIdAsync(string reactionTypeId)
        {
            return await _context.ReactionTypes
                .FirstOrDefaultAsync(rt => rt.Id == reactionTypeId);
        }

        public async Task UpdateAsync(ReactionModel model)
        {
            _context.ReactionActions.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ReactionModel>> GetByUserIdAsync(string id)
        {
            return await _context.ReactionActions
                .Include(r => r.ReactionType)
                .Include(r => r.User)
                .Where(ra => ra.UserId == id)
                .ToListAsync();
        }

        public async Task<List<ReactionModel>> GetByEntityIdAsync(string entityId)
        {
            return await _context.ReactionActions
                .Include(r => r.ReactionType)
                .Include(r => r.User)
                .Where(ra => ra.EntityId == entityId)
                .ToListAsync();
        }

        public async Task<ReactionModel> GetByEntityAndUserIdAsync(string userId, string entityId)
        {
            return await _context.ReactionActions
                .Include(r => r.ReactionType)
                .Include(r => r.User)
                .FirstOrDefaultAsync(ra => ra.UserId == userId && ra.EntityId == entityId);
        }

        public async Task DeleteAsync(ReactionModel action)
        {
            _context.ReactionActions.Remove(action);
            await _context.SaveChangesAsync();
        }
    }
}