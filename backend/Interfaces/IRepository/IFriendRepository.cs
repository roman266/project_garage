using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IFriendRepository
    {
        Task<FriendModel> GetByIdAsync(string id);
        Task<List<FriendModel>> GetByUserIdAsync(string userId);
        Task<List<DisplayFriendDto>> GetFriendsAsync(string userId, string? lastRequestId, int limit);
        Task<List<DisplayFriendDto>> GetIncomingRequestsAsync(string userId, string? lastRequestId, int limit);
        Task<List<DisplayFriendDto>> GetOutcomingRequestsAsync(string userId, string? lastRequestId, int limit);
        Task CreateNewRequestAsync(FriendModel friend);
        Task UpdateRequestAsync(FriendModel friend);
        Task DeleteFriendAsync(FriendModel friend);
    }
}
