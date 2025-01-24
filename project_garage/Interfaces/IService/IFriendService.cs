using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IFriendService
    {
        Task<int> GetFriendsCount(string id);
        Task<bool> IsFriendAsync(string userId, string friendId);
        Task<bool> CanAddFriendAsync(string userId, string friendId);
        Task<FriendModel> GetByIdAsync(string id);
        Task<List<FriendModel>> GetByUserIdAsync(string id);
        Task<bool> SendFriendRequestAsync(string userId, string friendId);
        Task AcceptRequestAsync(string userId, string friendId);
        Task RejectOrDeleteAsync(string userId, string friendId);
    }
}
