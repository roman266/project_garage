﻿using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface IMessageService
    {
        Task SendMessageAsync(string messageText, string conversationId, string senderId, string senderName);
        Task ReadMessageAsync(string messageId);
        Task<List<MessageModel>> GetConversationMessagesAsync(string conversationId);
        Task<List<MessageModel>> GetMessagesByUserIdAsync(string userId);
        Task<List<MessageDto>> GetUserMessagesByConversationIdAsync(string conversationId, string userId);
        Task DeleteMessageAsync(string messageId);
        Task DeleteMessageForMeAsync(string messageId);
    }
}
