using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository
{
    public class ConversationRepository : IConversationRepository
    {
        ApplicationDbContext _context;

        public ConversationRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task CreateNewAsync(ConversationModel conversation)
        {
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task<ConversationModel> GetByIdAsync(string id)
        {
            var conversation = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == id);
            return conversation;
        }

        public async Task UpdateAsync(ConversationModel conversation)
        {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ConversationModel conversation)
        {
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
        }
    }
}
