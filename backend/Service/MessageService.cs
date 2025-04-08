using Microsoft.AspNetCore.SignalR;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using project_garage.Models.DTOs;
using project_garage.Models.ViewModels;

namespace project_garage.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationService _conversationService;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public MessageService(IMessageRepository messageRepository, IConversationService conversationService, IHubContext<ChatHub> chatHubContext)
        {
            _messageRepository = messageRepository;
            _conversationService = conversationService;
        }

        public async Task<SendMessageDto> AddMessageAsync(MessageOnCreationDto messageOnCreationDto, string senderId)
        {

            if (!await _conversationService.IsUserInConversationAsync(senderId, 
                messageOnCreationDto.ConversationId))
                throw new Exception($"User with id: {senderId} " + 
                    $"isn't in conversation with id: {messageOnCreationDto.ConversationId}");

            var message = await _messageRepository.CreateNewAsync(messageOnCreationDto, senderId);
            var sendMessageDto = MapMessage(message, senderId);

            return sendMessageDto;
        }

        private SendMessageDto MapMessage(MessageModel message, string senderId)
        {
            return new SendMessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Text = message.Text,
                ImageUrl = message.ImageUrl,
                SendedAt = message.SendedAt,
                IsEdited = message.IsEdited,
                IsReaden = message.IsReaden,
                IsVisible = message.IsVisible,
            };
        }

        public async Task<bool> ReadMessageAsync(string messageId, string currentUserId)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new ArgumentException("Wrong message id");
            var message = await _messageRepository.GetByIdAsync(messageId);

            if (message == null)
                throw new ArgumentException("No message with this id found");

            if (message.SenderId != currentUserId) 
            {
                message.IsReaden = true;
                await _messageRepository.UpdateAsync(message);
                return true;
            }

            return false;
        }

        public async Task<List<MessageDto>> GetPaginatedConversationMessagesAsync(string conversationId, string lastMessageId, int messageCountLimit, string userId)
        {
            if (string.IsNullOrEmpty(conversationId))
                throw new ArgumentException("Wrong conversation id");

            if (!await _conversationService.IsUserInConversationAsync(userId, conversationId))
                throw new InvalidOperationException($"User is not part of conversation {conversationId}");

            var messages = await _messageRepository.GetPaginatedMessagesByConversationId(userId, conversationId, lastMessageId, messageCountLimit);
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

        public async Task UpdateMessageTextAsync(string messageId, string messageText)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new ArgumentException("Invalid edited text");

            var message = await _messageRepository.GetByIdAsync(messageId);
            message.Text = messageText;
            message.IsEdited = true;
            await _messageRepository.UpdateAsync(message);
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
