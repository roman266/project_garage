using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Repository
{
    public class MessageRepository : IMessageRepository
    {
        ApplicationDbContext _context;
        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNewAsync(MessageModel messageModel)
        {
            _context.Messages.Add(messageModel);
            await _context.SaveChangesAsync();
        }

        public async Task<MessageModel> GetByIdAsync(string id)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            return message;
        }

        public async Task<List<MessageModel>> GetByUserIdAsync(string id)
        {
            var messages = await _context.Messages.Where(m => m.SenderId == id).ToListAsync();
            return messages;
        }

        public async Task<List<MessageModel>> GetByConversationId(string id)
        {
            var messages = await _context.Messages.Where(m => m.ConversationId == id).ToListAsync();
            return messages;
        }

        public async Task<List<MessageDto>> GetMessagesForUserByConversationIdAsync(string conversationId, string userId)
        {
            var formattedMessages = await _context.Messages
                .Where(msg => msg.ConversationId == conversationId)
                .OrderBy(msg => msg.SendedAt)
                .Select(msg => new MessageDto
            {
                Id = msg.Id,
                ConversationId = msg.ConversationId,
                SenderId = msg.SenderId,
                SenderName = msg.SenderName,
                Text = msg.Text,
                SendedAt = msg.SendedAt,
                IsReaden = msg.IsReaden,
                IsVisible = msg.IsVisible,
                IsCurrentUser = msg.SenderId == userId 
                }).ToListAsync();

            return formattedMessages;
        }

        public async Task UpdateAsync(MessageModel messageModel)
        {
            _context.Messages.Update(messageModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MessageModel messageModel)
        {
            _context.Messages.Remove(messageModel);
            await _context.SaveChangesAsync();
        }
    }
}
