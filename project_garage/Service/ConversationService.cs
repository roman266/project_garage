using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Service
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageService _messageService;

        public ConversationService(IConversationRepository conversationRepository, IMessageService messageService)
        {
            _conversationRepository = conversationRepository;
            _messageService = messageService;
        }

        public async Task CreateConversationAsync(string user1Id, string user2Id)
        {
            if (string.IsNullOrWhiteSpace(user1Id) && string.IsNullOrEmpty(user2Id))
                throw new Exception("No conversation between this users found");
            
            if (await CheckForExistance(user1Id, user2Id))
                throw new Exception("This conversation already exist");
            var conversation = new ConversationModel
            {
                Id = Guid.NewGuid().ToString(),
                User1Id = user1Id,
                User2Id = user2Id,
                StartedAt = DateTime.UtcNow
            };

            await _conversationRepository.CreateNewAsync(conversation);
        }

        public async Task<bool> CheckForExistance(string user1Id, string user2Id)
        {
            var conversations = await GetConversationByUserIdAsync(user1Id);
            var exist = conversations.Where(c => c.User1Id == user2Id || c.User2Id == user2Id);
            if (exist.Any())
                return true;
            return false;
        }

        public async Task<List<ConversationModel>> GetConversationByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Wrong user id");

            var conversation = await _conversationRepository.GetByUserIdAsync(userId);
            if (conversation != null)
                return conversation;

            throw new Exception("User don't have conversations yet");
        }

        public async Task<List<MessageModel>> GetMessagesForUserByConversationIdAsync(string conversationId, string userId)
        {
            if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(userId))
                throw new Exception("Wrong input data");

            var messages = await _messageService.GetConversationMessagesAsync(conversationId);
            var userMessages = messages.Where(c => c.SenderId == userId).ToList();
            return userMessages;

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

        public async Task DeleteConversationAsync(string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
                throw new Exception("Wrong conversation id");

            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            if (conversation == null)
                throw new Exception($"Not found conversation wit this id");

            await _conversationRepository.DeleteAsync(conversation);
        }

    }
}
