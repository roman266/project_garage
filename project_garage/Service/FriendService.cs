using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Service
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        public FriendService(IFriendRepository friendRepository) 
        {
            _friendRepository = friendRepository;
        }
        //user - curr user, friend - target user
        public async Task<bool> IsFriendAsync(string userId, string friendId)
        {
            var friends = await _friendRepository.GetByUserIdAsync(userId);
            foreach (var friend in friends) {
                if (friend.FriendId == friendId) 
                    return true; }
            return false;
        }

        public async Task<bool> CanAddFriendAsync(string userId, string friendId)
        {
            if (userId == friendId)
                return false;

            if (await IsFriendAsync(userId, friendId))
                return false;

            return true;

        }

        public async Task<FriendModel> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("Searching failed");
            var request = await _friendRepository.GetById(id);
            return request;
        }

        public async Task<List<FriendModel>> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User Id is null here");
            var friends = await _friendRepository.GetByUserIdAsync(userId);
            return friends;
        }

        public async Task<bool> SendFriendRequestAsync(string userId, string friendId)
        {
            if (!await CanAddFriendAsync(userId, friendId))
            {
                return false;
            }

            var friendRequest = new FriendModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FriendId = friendId,
                IsAccepted = false
            };

            await _friendRepository.CreateNewRequestAsync(friendRequest);
            return true;
        }

        public async Task AcceptRequestAsync(string userId, string friendId)
        {
            var list = await GetByUserIdAsync(userId);
            foreach (var friend in list)
            {
                if (friend.FriendId == friendId)
                {
                    friend.IsAccepted = true;
                    await _friendRepository.UpdateRequestAsync(friend);
                }
            }
        }

        public async Task RejectOrDeleteAsync(string userId, string friendId)
        {
            var list = await GetByUserIdAsync(userId);
            foreach (var friend in list)
            {
                if (friend.FriendId == friendId)
                {
                    await _friendRepository.DeleteFriendAsync(friend);
                }
            }
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
