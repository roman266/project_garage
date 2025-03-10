using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Service
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public async Task<ConversationModel> AddConversationAsync(bool isPrivate)
        {
            var conversation = new ConversationModel
            {
                Id = Guid.NewGuid().ToString(),
                IsPrivate = isPrivate,
                StartedAt = DateTime.UtcNow,
            };

            await _conversationRepository.CreateNewAsync(conversation);

            return conversation;
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

    }
}
