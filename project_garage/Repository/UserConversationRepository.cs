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
        
        public async Task<List<ConversationModel>> GetUserConversationsAsync(string userId)
        {
            var conversationIds = await _context.UserConversations
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.ConversationId)
                .ToListAsync();

            var conversations = new List<ConversationModel>();

            foreach (var conversationId in conversationIds)
            {
               var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId);

                if (conversation != null)
                {
                    conversations.Add(conversation);
                }
            }

            conversations.GroupBy(c => c.StartedAt);

            return conversations;
        }
    }
}
