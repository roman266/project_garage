using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;
using project_garage.Models.DTOs;

namespace project_garage.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;
        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreatePostAsync(PostModel post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task<PostModel> GetPostByIdAsync(string id)
        {
            return await _context.Posts
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("No post with this id");
        }

        public int GetUsersPostsCount(string userId)
        {
            var count = _context.Posts
                .Where(p => p.UserId == userId)
                .Count();
            return count;     
        }

        public async Task<List<DisplayPostDto>> GetPaginatedPostsByUserIdAsync(string userId, string? lastPostId, int limit)
        {
            var lastPostDate = _context.Posts
                .Where(p => p.Id == lastPostId)
                .Select(p => p.CreatedAt)
                .FirstOrDefault();

            var postsQuery = _context.Posts
                .Where(p => p.UserId == userId &&
                    (lastPostDate == DateTime.MinValue || p.CreatedAt < lastPostDate))
                .Select(p => new DisplayPostDto
                {
                    PostId = p.Id,
                    PostImageUrl = p.ImageUrl,
                    PostDescription = p.Description,
                    PostDate = p.CreatedAt,
                    SenderNickName = p.User.UserName,
                    SenderAvatarUlr = p.User.ProfilePicture,
                })
                .OrderByDescending(p => p.PostDate);
                

            var posts = await postsQuery
                .Take(limit)
                .ToListAsync();

            return posts;
        }

        public async Task UpdatePostAsync(PostModel post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(string postId)
        {
            var post = await GetPostByIdAsync(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}
