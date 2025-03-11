using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IPostRepository
    {
        Task CreatePostAsync(PostModel post);
        Task<PostModel> GetPostByIdAsync(string postId);
        Task<List<PostModel>> GetPostByUserId(string postId);
        Task UpdatePostAsync(PostModel post);
        Task DeletePostAsync(string postId);
        Task<List<PostModel>> GetPostsByUserIdAsync(string userId);
    }
}
