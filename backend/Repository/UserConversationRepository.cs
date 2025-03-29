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
            var query = _context.UserConversations
                .Where(uc => uc.UserId == userId)
                .Join(_context.Conversations, uc => uc.ConversationId, c => c.Id, (uc, c) => new { uc, c })
                .OrderByDescending(x => x.c.StartedAt);

            if (!string.IsNullOrEmpty(lastConversationId))
            {
                var lastConversation = await _context.Conversations
                    .Where(c => c.Id == lastConversationId)
                    .Select(c => c.StartedAt)
                    .FirstOrDefaultAsync();

                if (lastConversation != default)
                {
                    query = query.Where(x => x.c.StartedAt < lastConversation)
                        .OrderByDescending(x => x.c.StartedAt);
                }
            }

            var conversationData = await query.Take(limit)
                .Select(x => new
                {
                    x.c.Id,
                    x.c.IsPrivate,
                    x.c.StartedAt,
                    UserId = _context.UserConversations
                        .Where(uc => uc.ConversationId == x.c.Id && uc.UserId != userId)
                        .Select(uc => uc.UserId)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var userIds = conversationData.Select(x => x.UserId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var conversations = conversationData.Select(x => new ConversationDisplayDto
            {
                ConversationId = x.Id,
                IsPrivate = x.IsPrivate,
                StartedAt = x.StartedAt,
                UserName = users.TryGetValue(x.UserId, out var user) ? user.UserName : null,
                ProfilePictureUrl = users.TryGetValue(x.UserId, out user) ? user.ProfilePicture : null,
                ActiveStatus = users.TryGetValue(x.UserId, out user) ? user.ActiveStatus : null
            }).ToList();

            return conversations;
        }



    }
}
