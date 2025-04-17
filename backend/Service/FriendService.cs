using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository; 
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
            foreach (var friend in friends)
            {
                if (friend.FriendId == friendId)
                { 
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> IsRequestBetweenUsersExistAsync(string userId, string friendId)
        {
            var request = await _friendRepository.GetRequestByUsersIdAsync(userId, friendId);
            if (request == null)
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

        public async Task<List<DisplayFriendDto>> GetFriendsAsync(string userId, string? lastRequestId, int limit)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid userId");
            var requests = await _friendRepository.GetFriendsAsync(userId, lastRequestId, limit);

            return requests;
        }

        public async Task<List<DisplayFriendDto>> GetIncomingRequestsAsync(string userId, string? lastRequestId, int limit)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid userId");
            var requests = await _friendRepository.GetIncomingRequestsAsync(userId, lastRequestId, limit);
            return requests;
        }

        public async Task<List<DisplayFriendDto>> GetOutcomingRequestsAsync(string userId, string? lastRequestId, int limit)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid userId");
            var requests = await _friendRepository.GetOutcomingRequestsAsync(userId, lastRequestId, limit);
            return requests;
        }

        public async Task SendFriendRequestAsync(string userId, string friendId)
        {
            if (await IsRequestBetweenUsersExistAsync(userId, friendId))
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
            var list = await _friendRepository.GetByUserIdAcceptedAsync(userId);
            return list.Count;
        }

        public async Task<List<FriendModel>> GetByUserIdAsync(string userId)
        {
            return await _friendRepository.GetByUserIdAsync(userId);
        }
    }
}