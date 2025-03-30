using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;

namespace project_garage.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public MessageRepository(ApplicationDbContext context, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<MessageModel> CreateNewAsync(MessageOnCreationDto messageDto)
        {
            var message = new MessageModel
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = messageDto.ConversationId,
                SenderId = messageDto.SenderId,
                Text = messageDto.Text,
                ImageUrl = messageDto.ImageUrl,
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

        public async Task<List<MessageDto>> GetPaginatedMessagesByConversationId(string userId, string conversationId, string? lastMessageId, int messageCountLimit)
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

            var messageData = await query
                .Take(messageCountLimit)
                .Select( md => new
                {
                    md.Id,
                    md.IsVisible,
                    md.SendedAt,
                    md.SenderId,
                    md.IsReaden,
                    md.Text,
                    md.ImageUrl,
                }
                )
                .ToListAsync();

            var userIds = messageData
                .Select(md => md.SenderId)
                .Distinct()
                .ToList();

            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var messages = messageData.Select(md => new MessageDto
            {
                Id = md.Id,
                IsVisible = md.IsVisible,
                SendedAt = md.SendedAt,
                IsReaden = md.IsReaden,
                SenderId = md.SenderId,
                IsCurrentUser = md.SenderId == userId,
                SenderName = users.TryGetValue(md.SenderId, out var user) ? user.UserName : null,
                SenderProfilePicture = users.TryGetValue(md.SenderId, out user) ? user.ProfilePicture : null,
                Text = md.Text,
                ImageUrl = md.ImageUrl,
            })
            .OrderByDescending(md => md.SendedAt)
            .ToList();
            
            return messages;
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
