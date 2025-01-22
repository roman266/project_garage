using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService
{
    public interface IPostService
    {
        Task<int> GetCountOfPosts(string id);

        Task CreatePostAsync(PostModel post);
    }
}
