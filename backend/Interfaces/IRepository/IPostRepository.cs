using project_garage.Models.DbModels;
using project_garage.Models.DTOs;

namespace project_garage.Interfaces.IRepository
{
    public interface IPostRepository
    {
        Task CreatePostAsync(PostModel post);
        Task<PostModel> GetPostByIdAsync(string postId);
        Task UpdatePostAsync(PostModel post);
        int GetUsersPostsCount(string userId);
        Task DeletePostAsync(string postId);
        Task<List<DisplayPostDto>> GetPaginatedPostsByUserIdAsync(string userId, string? lastPostId, int limit);
        Task<List<PostModel>> GetPostsByUserIdsAsync(List<string> userIds);
    }
}
