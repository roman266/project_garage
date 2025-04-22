using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Models.DTOs;

namespace project_garage.Service
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserConversationRepository _userConversationRepository;

        public ConversationService(IConversationRepository conversationRepository, IUserConversationRepository userConversationRepository)
        {
            _conversationRepository = conversationRepository;
            _userConversationRepository = userConversationRepository;
        }

        public async Task<ConversationModel> AddConversationAsync(bool isPrivate)
        {
            var conversation = new ConversationModel
            {
                Id = Guid.NewGuid().ToString(),
                IsPrivate = isPrivate,
                StartedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            await _conversationRepository.CreateNewAsync(conversation);

            return conversation;
        }

        public async Task UpdateLastMessageAsync(string conversationId)
        {
            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            if(conversationId == null)
                throw new ArgumentException("Conversation with this id does not exist");

            conversation.LastUpdatedAt = DateTime.UtcNow;
            await _conversationRepository.UpdateAsync(conversation);
        }

        public async Task<ConversationModel> GetConversationByIdAsync(string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
                throw new Exception("Wrong conversation id");

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            if (conversation != null)
                return conversation;

            throw new Exception("No conversation with this id found");
        }

        public async Task<List<string>> GetConversationMembersIdsAsync(string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
                throw new ArgumentException("Invalid conversationId");

            var users = await _userConversationRepository.GetConversationMembersAsync(conversationId);

            if (!users.Any())
                throw new InvalidOperationException("Conversation does not exist");

            var userIds = users.Select(x => x.Id).ToList();

            return userIds;
        }

        public async Task CreatePrivateConversationBetweenUsersAsync(string userId, string anotherUserId)
        {
            if (await _userConversationRepository.ExistsPrivateConversationAsync(userId, anotherUserId))
                throw new InvalidOperationException("Those users already have private conversation");

            var conversation = await AddConversationAsync(true);

            var firstUserModel = new UserConversationModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                ConversationId = conversation.Id,
            };

            var secondUserModel = new UserConversationModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = anotherUserId,
                ConversationId = conversation.Id,
            };

            await _userConversationRepository.AddUserToConversationAsync(firstUserModel);
            await _userConversationRepository.AddUserToConversationAsync(secondUserModel);
        }

        public async Task<bool> IsUserInConversationAsync(string userId, string conversationId)
        {
            return await _userConversationRepository.IsUserInConversationAsync(userId, conversationId);
        }

        public async Task<List<ConversationDisplayDto>> GetPaginatedConversationsByUserIdAsync(string userId, string? lastConversationId, int limit)
        {
            var conversations = await _userConversationRepository.GetPaginatedUserConversationsAsync(userId, lastConversationId, limit);
            return conversations;
        }

        public async Task<bool> CheckConversationExistance(string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId)) 
                throw new ArgumentException("ConversationId cannot be null");

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);

            return conversation != null;
        }
        
        public async Task DeleteConversationAsync(string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
                throw new Exception("Wrong conversation id");

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            if (conversation == null)
                throw new Exception($"Not found conversation wit this id");

            await _conversationRepository.DeleteAsync(conversation);
        }

        public async Task<string> GetPrivateConversationIdByFriendIdAsync(string userId, string friendId)
        {
            var conversationId = await _userConversationRepository.GetPrivateConversationBetweenUsersAsync(userId, friendId);
            return conversationId;
        }
    }
}
