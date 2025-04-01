using project_garage.Models.DbModels;
using project_garage.Models.DTOs;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface IPostService
    {
        int GetCountOfPosts(string id);
        Task CreatePostAndUploadImageToCloudAsync(string userId, PostOnCreationDto postDto);
        Task<List<DisplayPostDto>> GetPaginatedPostsByUserIdAsync(string userId, string? lastPostId, int limit);
        Task<PostModel> GetPostByIdAsync(string id);
        Task UpdatePostAsync(EditPostDto post);
        Task DeletePostAsync(string id);
    }
}
