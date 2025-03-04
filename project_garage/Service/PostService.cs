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

        public async Task<List<PostModel>> GetPostsByUserIdAsync(string userId)
        {
            return await _postRepository.GetPostsByUserIdAsync(userId);
        }
        public async Task<PostModel> GetPostByIdAsync(Guid id)
        {
            return await _postRepository.GetPostByIdAsync(id);
        }

        public async Task UpdatePostAsync(PostModel post)
        {
            await _postRepository.UpdatePostAsync(post);
        }

        public async Task DeletePostAsync(Guid id)
        {
            await _postRepository.DeletePostAsync(id);
        }
        public async Task AddImagesToPostAsync(Guid postId, List<string> imageUrls)
        {
            await _postRepository.AddImagesToPostAsync(postId, imageUrls);
        }

    }
}
