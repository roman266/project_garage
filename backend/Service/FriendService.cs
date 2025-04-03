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
            var request = await _friendRepository.GetByIdAsync(id);
            return request;
        }

        public async Task<List<FriendModel>> GetFriendsAsync(string userId, string? lastRequestId, int limit)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid userId");
            var requests = await _friendRepository.GetFriendsAsync(userId, lastRequestId, limit);

            return requests;
        }

        public async Task<List<FriendModel>> GetIncomingRequestsAsync(string userId, string? lastRequestId, int limit)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid userId");
            var requests = await _friendRepository.GetIncomingRequestsAsync(userId, lastRequestId, limit);
            return requests;
        }

        public async Task<List<FriendModel>> GetOutcomingRequestsAsync(string userId, string? lastRequestId, int limit)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid userId");
            var requests = await _friendRepository.GetOutcomingRequestsAsync(userId, lastRequestId, limit);
            return requests;
        }

        public async Task SendFriendRequestAsync(string userId, string friendId)
        {
            if (!await CanAddFriendAsync(userId, friendId))
                throw new InvalidOperationException("Users already are in friendship");

            var friendRequest = new FriendModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FriendId = friendId,
                IsAccepted = false,
            };

            await _friendRepository.CreateNewRequestAsync(friendRequest);
        }

        public async Task AcceptRequestAsync(string requestId)
        {
            var request = await _friendRepository.GetByIdAsync(requestId);
            if (request == null)
                throw new ArgumentException("Request with this id doesn't exist");

            if (request.IsAccepted)
                throw new InvalidOperationException("Request already accepted");

            request.IsAccepted = true;
            await _friendRepository.UpdateRequestAsync(request);
        }

        public async Task RejectOrDeleteAsync(string requestId)
        {
            var request = await _friendRepository.GetByIdAsync(requestId);
            await _friendRepository.DeleteFriendAsync(request);
        }

        public async Task<int> GetFriendsCount(string userId) 
        {
                var list = await _friendRepository.GetByUserIdAsync(userId);
                return list.Count;
        }
    }
}
