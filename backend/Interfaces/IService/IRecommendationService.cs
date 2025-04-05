using project_garage.Models.DbModels;
using project_garage.Models.Enums;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface IRecommendationService
    {
        Task<object> GetUserFeedAsync(string userId, string? lastRequestId, int limit);
        Task<List<PostModel>> GetRecommendedPostsAsync(string userId);
    }
}