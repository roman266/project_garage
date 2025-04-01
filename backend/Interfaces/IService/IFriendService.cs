using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IFriendService
    {
        Task<int> GetFriendsCount(string id);
        Task<bool> IsFriendAsync(string userId, string friendId);
        Task<bool> CanAddFriendAsync(string userId, string friendId);
        Task<List<FriendModel>> GetFriendsAsync(string userId, string? lastFriendId, int limit);
        Task<List<FriendModel>> GetIncomingRequestsAsync(string userId, string? lastFriendId, int limit);
        Task<List<FriendModel>> GetOutcomingRequestsAsync(string userId, string? lastFriendId, int limit);
        Task<FriendModel> GetByIdAsync(string id);
        Task SendFriendRequestAsync(string userId, string friendId);  
        Task AcceptRequestAsync(string requestId);
        Task RejectOrDeleteAsync(string requestId);
        Task<List<FriendModel>> GetFriendsByUserIdAsync(string userId);
        Task<List<FriendModel>> GetByUserIdAsync(string userId);
    }
}
