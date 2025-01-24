using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository
{
    public interface IPostRepository
    {
        Task CreatePostAsync(PostModel post);
        Task<PostModel> GetPostByIdAsync(Guid id);
        Task<List<PostModel>> GetPostByUserId(string id);
        Task UpdatePostAsync(PostModel post);
        Task DeletePostAsync(Guid id);
    }
}
