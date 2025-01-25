using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;

namespace project_garage.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task SendMessageAsync(string messageText, string conversationId, string senderId, string senderName)
        {
            if (string.IsNullOrEmpty(messageText) || messageText.LongCount() > 1000 || string.IsNullOrEmpty(conversationId) ||
            string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(senderName))
                throw new Exception("Somthing went wrong");

            var message = new MessageModel
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                SenderId = senderId,
                SenderName = senderName,
                Text = messageText,
                SendedAt = DateTime.UtcNow,
                IsReaden = false,
                IsVisible = true
            };

            await _messageRepository.CreateNewAsync(message);
        }

        public async Task ReadMessageAsync(string messageId)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new Exception("Wrong message id");
            var message = await _messageRepository.GetByIdAsync(messageId);

            if (message == null)
                throw new Exception("No message with this id found");

            message.IsReaden = true;
            await _messageRepository.UpdateAsync(message);
        }

        public async Task<List<MessageModel>> GetConversationMessagesAsync(string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
                throw new Exception("Wrong conversation id");

            var messages = await _messageRepository.GetByConversationId(conversationId);

            if (messages == null)
                throw new Exception("No conversation with this id");

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
