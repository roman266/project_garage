using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_garage.Service
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IPostService _postService;
        private readonly IFriendService _friendService;
        private readonly IUserInterestService _userInterestService;
        private readonly IUserService _userService;

        public RecommendationService(
            IPostService postService,
            IFriendService friendService,
            IUserInterestService userInterestService,
            IUserService userService)
        {
            _postService = postService;
            _friendService = friendService;
            _userInterestService = userInterestService;
            _userService = userService;
        }

        public async Task<object> GetUserFeedAsync(string userId, string? lastRequestId, int limit)
        {
            var recommendedPosts = await GetRecommendedPostsAsync(userId);
            var allPosts = recommendedPosts
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            Console.WriteLine($"Total recommended posts: {allPosts.Count}");

            if (!string.IsNullOrEmpty(lastRequestId) && int.TryParse(lastRequestId, out int lastId))
            {
                var lastIndex = allPosts.FindIndex(p => p.Id == lastId.ToString());
                if (lastIndex != -1)
                {
                    allPosts = allPosts.Skip(lastIndex + 1).ToList();
                }
            }

            int totalPages = (int)Math.Ceiling((double)allPosts.Count / limit);
            var paginatedPosts = allPosts.Take(limit).ToList();

            var user = await _userService.GetByIdAsync(userId) ?? new UserModel();

            var formattedPosts = paginatedPosts.Select(p => new
            {
                Id = p.Id,
                Content = p.Description,
                CreatedAt = p.CreatedAt,
                Author = p.User != null ? new
                {
                    Id = p.User.Id.ToString(),
                    Username = p.User.UserName ?? "Unknown",
                    FirstName = p.User.FirstName ?? "",
                    LastName = p.User.LastName ?? "",
                    ProfilePicture = p.User.ProfilePicture ?? ""
                } : new
                {
                    Id = "0",
                    Username = "Unknown",
                    FirstName = "",
                    LastName = "",
                    ProfilePicture = ""
                }
            }).ToList();

            var response = new
            {
                success = true,
                data = new
                {
                    user = new
                    {
                        Id = user.Id,
                        Username = user.UserName ?? "Unknown",
                        FirstName = user.FirstName ?? "",
                        LastName = user.LastName ?? "",
                        ProfilePicture = user.ProfilePicture ?? ""
                    },
                    posts = formattedPosts,
                    totalPages
                }
            };

            foreach (var post in paginatedPosts)
            {
                if (post.User != null)
                {
                    post.User.Friends = null;
                    post.User.Posts = null;
                }
            }

            return response;
        }

        public async Task<List<PostModel>> GetRecommendedPostsAsync(string userId)
        {
            var userPosts = await _postService.GetPostsByUserIdsAsync(new List<string> { userId });
            var userCategories = new HashSet<string>(userPosts.Select(p => p.Category?.ToString() ?? ""));
            var friends = await _friendService.GetByUserIdAsync(userId);
            var friendIds = friends.Where(f => f.IsAccepted).Select(f => f.FriendId).ToList();

            var friendsOfFriends = new HashSet<string>();
            foreach (var friendId in friendIds)
            {
                var fOfF = await _friendService.GetByUserIdAsync(friendId);
                friendsOfFriends.UnionWith(fOfF.Where(f => f.IsAccepted).Select(f => f.FriendId));
            }
            friendsOfFriends.ExceptWith(friendIds);
            friendsOfFriends.Remove(userId);

            var userInterests = await _userInterestService.GetUserInterestAsync(userId);
            var userInterestSet = new HashSet<string>(userInterests.Select(i => i.Interest.ToString()));

            Console.WriteLine($"Friends of friends count: {friendsOfFriends.Count}");
            Console.WriteLine($"User interests: {string.Join(", ", userInterestSet)}");
            Console.WriteLine($"User categories: {string.Join(", ", userCategories)}");

            if (!friendsOfFriends.Any())
            {
                Console.WriteLine("No friends of friends found.");
                return new List<PostModel>();
            }

            var userScores = new Dictionary<string, double>();
            foreach (var potentialUser in friendsOfFriends)
            {
                var potentialUserFriends = await _friendService.GetByUserIdAsync(potentialUser);
                var potentialFriendSet = new HashSet<string>(potentialUserFriends.Select(f => f.FriendId));

                var intersection = friendIds.Intersect(potentialFriendSet).Count();
                var union = friendIds.Union(potentialFriendSet).Count();
                double jaccardIndex = union == 0 ? 0 : (double)intersection / union;

                var potentialUserInterests = await _userInterestService.GetUserInterestAsync(potentialUser);
                var potentialInterestSet = new HashSet<string>(potentialUserInterests.Select(i => i.Interest.ToString()));

                double interestScore = userInterestSet.Any() ? (double)userInterestSet.Intersect(potentialInterestSet).Count() / userInterestSet.Count : 0;
                double finalScore = jaccardIndex + (interestScore * 0.5);

                userScores[potentialUser] = finalScore;
            }

            var groupedUsers = userScores
                .GroupBy(x => Math.Round(x.Value, 1))
                .ToDictionary(g => g.Key, g => g.Select(u => u.Key).ToList());

            var recommendedPosts = new List<PostModel>();
            foreach (var group in groupedUsers.OrderByDescending(g => g.Key))
            {
                var usersInGroup = group.Value;
                var posts = await _postService.GetPostsByUserIdsAsync(usersInGroup);

                var filteredPosts = userInterestSet.Any()
                    ? posts.Where(p => p.UserId != userId && userInterestSet.Contains(p.Category?.ToString() ?? ""))
                    : posts.Where(p => p.UserId != userId);

                Console.WriteLine($"Group {group.Key}, Users: {usersInGroup.Count}, Posts: {posts.Count}, Filtered: {filteredPosts.Count()}");

                var selectedPosts = filteredPosts
                    .GroupBy(p => p.UserId)
                    .SelectMany(g => g.Take(3))
                    .ToList();
                recommendedPosts.AddRange(selectedPosts);
            }

            return recommendedPosts.OrderBy(_ => Guid.NewGuid()).ToList();
        }
    }
}