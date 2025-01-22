using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Repository;

namespace project_garage.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        public PostService(IPostRepository postRepository) 
        { 
            _postRepository = postRepository;
        }

        public async Task<int> GetCountOfPosts(string id) 
        {
            try
            {
                var posts = await _postRepository.GetPostByUserId(id);
                var count = posts.Count();
                return count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task CreatePostAsync(PostModel post)
        {
            await _postRepository.CreatePostAsync(post);
        }
    }
}
