using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IPostService
    {
        Task<int> GetCountOfPosts(string id);

        Task CreatePostAsync(PostModel post);

        Task<List<PostModel>> GetPostsByUserIdAsync(string userId);
        Task<PostModel> GetPostByIdAsync(Guid id);
        Task UpdatePostAsync(PostModel post);
        Task DeletePostAsync(Guid id);
        Task AddImagesToPostAsync(Guid postId, List<string> imageUrls);

    }
}
