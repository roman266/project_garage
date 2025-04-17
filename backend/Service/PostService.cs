using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;
using project_garage.Models.DTOs;

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

        public int GetCountOfPosts(string userId) 
        {
            var count = _postRepository.GetUsersPostsCount(userId);
            return count;
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
                CategoryId = postDto.CategoryId,
                Category = postDto.Category,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _postRepository.CreatePostAsync(post);
        }

        public async Task<List<DisplayPostDto>> GetPaginatedPostsByUserIdAsync(string userId, string? lastPostId, int limit)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();
            
            var posts = await _postRepository.GetPaginatedPostsByUserIdAsync(userId, lastPostId, limit);
            return posts;
        }

        public async Task<PostModel> GetPostByIdAsync(string postId)
        {
            return await _postRepository.GetPostByIdAsync(postId);
        }

        public async Task UpdatePostAsync(EditPostDto editPostDto)
        {
            var post = await _postRepository.GetPostByIdAsync(editPostDto.PostId);
            post.Description = string.IsNullOrEmpty(editPostDto.Description) ? post.Description : editPostDto.Description;
            await _postRepository.UpdatePostAsync(post);
        }

        public async Task DeletePostAsync(string postId)
        {
            await _postRepository.DeletePostAsync(postId);
        }

        public async Task<List<PostModel>> GetPostsByUserIdsAsync(List<string> userIds)
        {
            return await _postRepository.GetPostsByUserIdsAsync(userIds);
        }
    }
}