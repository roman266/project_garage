using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Repository;
using project_garage.Models.ViewModels;

namespace project_garage.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICloudinaryService _cloudinaryService;
        public PostService(IPostRepository postRepository, ICloudinaryService cloudinaryService) 
        { 
            _postRepository = postRepository;
            _cloudinaryService = cloudinaryService;
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

        public async Task CreatePostAndUploadImageToCloudAsync(string userId, PostOnCreationDto postDto)
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(postDto.Image);

            var post = new PostModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Description = postDto.Description,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _postRepository.CreatePostAsync(post);
        }

        public async Task<List<PostModel>> GetPostsByUserIdAsync(string userId)
        {
            return await _postRepository.GetPostsByUserIdAsync(userId);
        }
        public async Task<PostModel> GetPostByIdAsync(string postId)
        {
            return await _postRepository.GetPostByIdAsync(postId);
        }

        public async Task UpdatePostAsync(PostModel post)
        {
            await _postRepository.UpdatePostAsync(post);
        }

        public async Task DeletePostAsync(string postId)
        {
            await _postRepository.DeletePostAsync(postId);
        }
    }
}
