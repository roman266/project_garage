using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository
{
    public class UserConversationRepository : IUserConversationRepository
    {
        private readonly ApplicationDbContext _context;

        public UserConversationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsUserInConversationAsync(string userId, string conversationId)
        {
            var conversation = await _context.UserConversations.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ConversationId == conversationId);

            return conversation != null;
        }

        public async Task AddUserToConversationAsync(UserConversationModel userConversation)
        {
            _context.UserConversations.Add(userConversation);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromConversationAsync(string userId, string conversationId)
        {
            var userConversation = await _context.UserConversations.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ConversationId == conversationId);

            if (userConversation != null)
            {
                _context.UserConversations.Remove(userConversation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsPrivateConversationAsync(string userId, string anotherUserId)
        {
            return await _context.UserConversations
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.Conversation)
                .Where(c => c.IsPrivate) 
                .AnyAsync(c => c.UserConversations.Any(uc => uc.UserId == anotherUserId));
        }

        public async Task<List<ConversationModel>> GetPaginatedUserConversationsAsync(
            string userId, string? lastConversationId, int limit)
        {
            var query = _context.UserConversations
                .Where(uc => uc.UserId == userId)
                .Join(_context.Conversations, uc => uc.ConversationId, c => c.Id, (uc, c) => c)
                .OrderByDescending(c => c.StartedAt);

            if (!string.IsNullOrEmpty(lastConversationId))
            {
                var lastConversation = await _context.Conversations
                    .Where(c => c.Id == lastConversationId)
                    .Select(c => c.StartedAt)
                    .FirstOrDefaultAsync();

                if (lastConversation != default)
                {
                    query = query.Where(c => c.StartedAt < lastConversation).OrderByDescending(c => c.StartedAt);
                }
            }

            return await query.Take(limit).ToListAsync();
        }

    }
}
