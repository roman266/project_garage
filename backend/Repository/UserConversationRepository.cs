using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Models.DTOs;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using Microsoft.VisualBasic;

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

        public async Task<List<UserModel>> GetConversationMembersAsync(string conversationId)
        {
            var users = await _context.UserConversations
                .Where(uc => uc.ConversationId == conversationId)
                .Select(uc => uc.User)
                .ToListAsync();

            return users;
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

        public async Task<List<ConversationDisplayDto>> GetPaginatedUserConversationsAsync(
            string userId, string? lastConversationId, int limit)
        {
            var lastMessageDate = await _context.UserConversations
                .Where(uc => uc.ConversationId == lastConversationId)
                .Select(uc => uc.Conversation.LastUpdatedAt)
                .FirstOrDefaultAsync();

            var conversations = await _context.UserConversations
                .Where(uc => uc.UserId == userId &&
                    (lastMessageDate == DateTime.MinValue || uc.Conversation.LastUpdatedAt < lastMessageDate))
                .OrderByDescending(uc => uc.Conversation.LastUpdatedAt)
                .Take(limit)
                .Include(uc => uc.Conversation)
                .Include(uc => uc.Conversation.UserConversations)
                .ThenInclude(uc => uc.User)
                .Select(uc => new
                {
                    uc.Conversation,
                    OtherUser = uc.Conversation.UserConversations
                        .Where(u => u.UserId != userId)
                        .Select(u => u.User)
                        .FirstOrDefault()
                })
                .Select(c => new ConversationDisplayDto
                {
                    ConversationId = c.Conversation.Id,
                    ProfilePictureUrl = c.OtherUser.ProfilePicture,
                    UserName = c.OtherUser.UserName,
                    ActiveStatus = c.OtherUser.ActiveStatus,
                    IsPrivate = c.Conversation.IsPrivate,
                    StartedAt = c.Conversation.StartedAt,
                    LastUpdatedAt = c.Conversation.LastUpdatedAt,
                })
                .ToListAsync();

            return conversations;
        }
    }
}
