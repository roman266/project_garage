﻿using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IConversationRepository
    {
        Task CreateNewAsync(ConversationModel conversation);
        Task<List<ConversationModel>> GetByUserIdAsync(string userId);
        Task<ConversationModel> GetByIdAsync(string id);
        Task UpdateAsync(ConversationModel conversation);
        Task DeleteAsync(ConversationModel conversation);
    }
}
