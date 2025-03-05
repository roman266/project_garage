using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

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

        public async Task<PostModel> GetPostByIdAsync(Guid id)
        {
            return await _context.Posts
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new Exception("No post with this id");
        }


        public async Task<List<PostModel>> GetPostByUserId(string id)
        {
            var posts = await _context.Posts
                .Where(x => x.UserId == id)
                .ToListAsync();

            if (posts == null || !posts.Any())
            {
                throw new Exception("No posts found for this user");
            }

            return posts;
        }

        public async Task<List<PostModel>> GetPostsByUserIdAsync(string userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdatePostAsync(PostModel post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(Guid id)
        {
            var post = await GetPostByIdAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddImagesToPostAsync(Guid postId, List<string> imageUrls)
        {
        //    var images = imageUrls.Select(url => new PostImageModel { PostId = postId, ImageUrl = url }).ToList();
        //    await _context.PostImages.AddRangeAsync(images);
        //    await _context.SaveChangesAsync();
        }
    }
}
