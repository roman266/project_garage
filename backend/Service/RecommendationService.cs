using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Repository;
using project_garage.Models.ViewModels;

namespace project_garage.Service
{ 
    public class RecommendationService : IRecommendationService
    {
        private readonly IPostService _postService;
        private readonly IFriendService _friendService;
        private readonly IUserInterestService _userInterestService;  

        public RecommendationService(
            IPostService postService, 
            IFriendService friendService, 
            IUserInterestService userInterestService)
        {
            _postService = postService;
            _friendService = friendService;
            _userInterestService = userInterestService;
        }
   
        public async Task<List<PostModel>> GetRecommendedPostsAsync(string userId)
        {
            Console.WriteLine($"Fetching posts for user: {userId}"); // -----
            
            var userPosts = await _postService.GetPostsByUserIdsAsync(new List<string> { userId });
            var userCategories = new HashSet<string>(userPosts.Select(p => p.Category.ToString()));
            
            var friends = await _friendService.GetByUserIdAsync(userId);
            var friendIds = friends.Where(f => f.IsAccepted).Select(f => f.FriendId).ToList();
            Console.WriteLine($"Friends with IsAccepted=true: {friendIds.Count}");

            var friendsOfFriends = new HashSet<string>();
            foreach (var friendId in friendIds)
            {
                var fOfF = await _friendService.GetByUserIdAsync(friendId);
                friendsOfFriends.UnionWith(fOfF.Where(f => f.IsAccepted).Select(f => f.FriendId));
            }
            friendsOfFriends.ExceptWith(friendIds);
            Console.WriteLine($"Before removing userId: {string.Join(", ", friendsOfFriends)}");
            friendsOfFriends.Remove(userId);
            Console.WriteLine($"After removing userId: {string.Join(", ", friendsOfFriends)}"); 

            var userInterests = await _userInterestService.GetUserInterestAsync(userId);
            var userInterestSet = new HashSet<string>(userInterests.Select(i => i.Interest.ToString()));

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

                double interestScore = (double)userInterestSet.Intersect(potentialInterestSet).Count() / userInterestSet.Count;
                double finalScore = jaccardIndex + (interestScore * 0.5); 

                userScores[potentialUser] = finalScore;
            }

            var groupedUsers = userScores
                .GroupBy(x => Math.Round(x.Value, 1))
                .ToDictionary(g => g.Key, g => g.Select(u => u.Key).ToList());

            var recommendedPosts = new List<PostModel>();

            foreach (var group in groupedUsers)
            {
                var usersInGroup = group.Value;
                var posts = await _postService.GetPostsByUserIdsAsync(usersInGroup);

                // Фільтруємо пости, виключаючи ті, що мають категорії ваших постів
                var filteredPosts = posts.Where(p => 
                    userInterestSet.Contains(p.Category.ToString()) && 
                    !userCategories.Contains(p.Category.ToString())    
                ).ToList();

                var selectedPosts = filteredPosts.GroupBy(p => p.UserId)
                                                .SelectMany(g => g.Take(3))
                                                .ToList();
                recommendedPosts.AddRange(selectedPosts);
            }

            return recommendedPosts.OrderBy(_ => Guid.NewGuid()).ToList(); 
        }
    }
}