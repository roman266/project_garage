using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IConversationService
    {
        Task<ConversationModel> AddConversationAsync(bool isPrivate);
        Task<ConversationModel> GetConversationByIdAsync(string id);
        Task DeleteConversationAsync(string conversationId);
    }
}
