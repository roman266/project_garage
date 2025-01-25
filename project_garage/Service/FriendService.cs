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
            if (userId == friendId)
                return true;
            var friends = await _friendRepository.GetByUserIdAsync(userId);
            if (!friends.Any())
                return false;
            foreach (var friend in friends) {
                if (friend.FriendId == friendId) 
                    return true; }
            return false;
        }

        public async Task<FriendModel> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new InvalidDataException("Invalid data");
            var request = await _friendRepository.GetById(id);
            return request;
        }

        public async Task<List<FriendModel>> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new InvalidDataException("Invalid data");
            var friends = await _friendRepository.GetByUserIdAsync(userId);
            return friends;
        }

        public async Task SendFriendRequestAsync(string userId, string friendId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(friendId))
                throw new InvalidDataException("Invalida data");

            if (await IsFriendAsync(userId, friendId))
                throw new Exception("User's already are in friendship");
            

            var friendRequest = new FriendModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FriendId = friendId,
                IsAccepted = false
            };

            await _friendRepository.CreateNewRequestAsync(friendRequest);
        }

        public async Task AcceptRequestAsync(string userId, string friendId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(friendId))
                throw new InvalidDataException("Invalida data");
            
            bool check = false;

            var list = await GetByUserIdAsync(userId);
            foreach (var friend in list)
            {
                if (friend.FriendId == friendId)
                {
                    check = true;
                    friend.IsAccepted = true;
                    await _friendRepository.UpdateRequestAsync(friend);
                }
            }
            if (!check)
                throw new Exception("You can't accept your own requests");
        }

        public async Task RejectOrDeleteAsync(string userId, string friendId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(friendId))
                throw new InvalidDataException("Invalida data");
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
            if (string.IsNullOrEmpty(id))
                throw new InvalidDataException("Invalida data");

            try
            {
                var list = await _friendRepository.GetByUserIdAsync(id);
                var accepted = list.Where(x => x.IsAccepted == true).ToList();
                return accepted.Count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
