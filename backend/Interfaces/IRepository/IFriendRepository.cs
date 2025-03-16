using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IFriendRepository
    {
        Task<FriendModel> GetByIdAsync(string id);
        Task<List<FriendModel>> GetByUserIdAsync(string userId);
        Task<List<FriendModel>> GetFriendsAsync(string userId, string? lastRequestId, int limit);
        Task<List<FriendModel>> GetIncomingRequestsAsync(string userId, string? lastRequestId, int limit);
        Task<List<FriendModel>> GetOutcomingRequestsAsync(string userId, string? lastRequestId, int limit);
        Task CreateNewRequestAsync(FriendModel friend);
        Task UpdateRequestAsync(FriendModel friend);
        Task DeleteFriendAsync(FriendModel friend);
    }
}
