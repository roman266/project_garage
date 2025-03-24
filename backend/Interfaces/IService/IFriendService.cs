using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface IFriendService
    {
        Task<int> GetFriendsCount(string id);
        Task<bool> IsFriendAsync(string userId, string friendId);
        Task<bool> CanAddFriendAsync(string userId, string friendId);
        Task<List<DisplayFriendDto>> GetFriendsAsync(string userId, string? lastFriendId, int limit);
        Task<List<DisplayFriendDto>> GetIncomingRequestsAsync(string userId, string? lastFriendId, int limit);
        Task<List<DisplayFriendDto>> GetOutcomingRequestsAsync(string userId, string? lastFriendId, int limit);
        Task<FriendModel> GetByIdAsync(string id);
        Task SendFriendRequestAsync(string userId, string friendId);
        Task AcceptRequestAsync(string requestId);
        Task RejectOrDeleteAsync(string requestId);
    }
}
