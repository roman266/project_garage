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

        public async Task<MessageModel> CreateNewAsync(MessageOnCreationDto messageDto)
        {
            var userName = await _context.Users
                .Where(u => u.Id == messageDto.SenderId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            var message = new MessageModel
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = messageDto.ConversationId,
                SenderId = messageDto.SenderId,
                SenderName = userName,
                Text = messageDto.Text,
                SendedAt = DateTime.UtcNow,
                IsReaden = false,
                IsVisible = true,
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return message;
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

        public async Task<List<MessageModel>> GetPaginatedMessagesByConversationId(string conversationId, string? lastMessageId, int messageCountLimit)
        {
            var query = _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SendedAt);

            if (!string.IsNullOrEmpty(lastMessageId))
            {
                var lastMessage = await _context.Messages.FindAsync(lastMessageId);
                if (lastMessage != null)
                {
                    query = query.Where(m => m.SendedAt < lastMessage.SendedAt)
                         .OrderByDescending(m => m.SendedAt);
                }
            }

            return await query.Take(messageCountLimit).ToListAsync();
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
