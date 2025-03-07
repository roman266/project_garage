using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Service
{
    public class UserConversationService : IUserConversationService
    {
        private readonly IUserConversationRepository _userConversationRepository;

        public UserConversationService(IUserConversationRepository userConversationRepository)
        {
            _userConversationRepository = userConversationRepository;
        }

        public async Task<UserConversationModel> AddUserToConversationAsync(string userId, string conversationId)
        {
            if (await IsUserInConversationAsync(userId, conversationId))
                throw new Exception($"User with id {userId} is in conversation {conversationId} already");

            var userConversation = new UserConversationModel 
            { 
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow,
            };

            await _userConversationRepository.AddUserToConversationAsync(userConversation);

            return userConversation;
        }

        public async Task<bool> IsUserInConversationAsync(string userId, string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(userId))
                throw new ArgumentException("Incorrect user or conversationId");

            var result = await _userConversationRepository.IsUserInConversationAsync(userId, conversationId);

            return result;
        }

        public async Task<List<ConversationModel>> GetUserConversationsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Incorrect userId");

            var conversationList = await _userConversationRepository.GetUserConversationsAsync(userId);

            return conversationList;
        }

        public async Task RemoveUserFromConversationAsync(string userId, string conversationId)
        {
            await _userConversationRepository.RemoveUserFromConversationAsync(userId, conversationId);
        }
    }
}
