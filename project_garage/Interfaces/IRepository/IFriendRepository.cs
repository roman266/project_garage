using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IFriendRepository
    {
        Task<FriendModel> GetById(string id);
        Task<List<FriendModel>> GetByUserIdAsync(string id);
        Task SendFriendRequestAsync(FriendModel friend);
        Task AcceptRequestAsync(FriendModel friend);
        Task RejectRequestAsync(FriendModel friend);
        Task RemoveFriendAsync(FriendModel friend);
    }
}
