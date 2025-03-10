using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationService _conversationService;

        public MessageService(IMessageRepository messageRepository, IConversationService conversationService)
        {
            _messageRepository = messageRepository;
            _conversationService = conversationService;
        }

        public async Task<MessageModel> AddMessageAsync(MessageOnCreationDto messageOnCreationDto)
        {

            if (!await _conversationService.IsUserInConversationAsync(messageOnCreationDto.SenderId, 
                messageOnCreationDto.ConversationId))
                throw new Exception($"User with id: {messageOnCreationDto.SenderId} " + 
                    $"isn't in conversation with id: {messageOnCreationDto.ConversationId}");

            var message = await _messageRepository.CreateNewAsync(messageOnCreationDto);
            
            return message;
        }

        public async Task ReadMessageAsync(string messageId)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new ArgumentException("Wrong message id");
            var message = await _messageRepository.GetByIdAsync(messageId);

            if (message == null)
                throw new ArgumentException("No message with this id found");

            message.IsReaden = true;
            await _messageRepository.UpdateAsync(message);
        }

        public async Task<List<MessageModel>> GetPaginatedConversationMessagesAsync(string conversationId, string lastMessageId, int messageCountLimit, string userId)
        {
            if (string.IsNullOrEmpty(conversationId))
                throw new ArgumentException("Wrong conversation id");

            if (!await _conversationService.IsUserInConversationAsync(userId, conversationId))
                throw new InvalidOperationException($"User is not part of conversation {conversationId}");

            var messages = await _messageRepository.GetPaginatedMessagesByConversationId(conversationId, lastMessageId, messageCountLimit);

            if (!messages.Any())
                throw new KeyNotFoundException("You dont have messages with this user");

            return messages;
        }

        public async Task<List<MessageModel>> GetMessagesByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Wrong user id");

            var messages = await _messageRepository.GetByUserIdAsync(userId);

            if(messages == null)
                throw new Exception("No messages for user found");

            return messages;
        }

        public async Task<List<MessageDto>> GetUserMessagesByConversationIdAsync(string conversationId, string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("Incorrect userId");

            if (string.IsNullOrEmpty(conversationId)) throw new ArgumentException("Incorrect conversationId");

            var messages = await _messageRepository.GetMessagesForUserByConversationIdAsync(conversationId, userId);

            return messages;
        }

        public async Task DeleteMessageAsync(string messageId) 
        {
            if (string.IsNullOrEmpty(messageId))
                throw new Exception("Wrong messagee id");
            var message = await _messageRepository.GetByIdAsync(messageId);

            if (message == null)
                throw new Exception("No message with this id found");

            await _messageRepository.DeleteAsync(message);
        }

        public async Task DeleteMessageForMeAsync(string messageId)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new Exception("Wrong message id");
            var message = await _messageRepository.GetByIdAsync(messageId);

            if (message == null)
                throw new Exception("No message with this id found");

            message.IsVisible = false;
            await _messageRepository.UpdateAsync(message);
        }
    }
}
