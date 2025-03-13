using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;

namespace project_garage.Interfaces.IService
{
    public interface IPostService
    {
        Task<int> GetCountOfPosts(string id);
        Task CreatePostAndUploadImageToCloudAsync(string userId, PostOnCreationDto postDto);
        Task<List<PostModel>> GetPostsByUserIdAsync(string userId);
        Task<PostModel> GetPostByIdAsync(string id);
        Task UpdatePostAsync(PostModel post);
        Task DeletePostAsync(string id);
    }
}
