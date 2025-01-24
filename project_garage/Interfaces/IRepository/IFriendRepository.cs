using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IFriendRepository
    {
        Task<FriendModel> GetById(string id);
        Task<List<FriendModel>> GetByUserIdAsync(string id);
        Task CreateNewRequestAsync(FriendModel friend);
        Task UpdateRequestAsync(FriendModel friend);
        Task DeleteFriendAsync(FriendModel friend);
    }
}
