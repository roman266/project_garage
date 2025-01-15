using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;

namespace project_garage.Service
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        public FriendService(IFriendRepository friendRepository) 
        {
            _friendRepository = friendRepository;
        }

        public async Task<int> GetFriendsCount(string id) 
        {
            try
            {
                var list = await _friendRepository.GetByUserIdAsync(id);
                return list.Count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
