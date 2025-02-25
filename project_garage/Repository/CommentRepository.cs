using Microsoft.EntityFrameworkCore;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository {
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommentModel> CreateCommentAsync(CommentModel comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<CommentModel> GetCommentByIdAsync(int commentId)
        {
            return await _context.Comments.FindAsync(commentId);
        }

        public async Task<IEnumerable<CommentModel>> GetCommentsByPostIdAsync(Guid postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .ToListAsync();
        }
        public async Task DeleteCommentAsync(CommentModel comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
        
        public async Task<CommentModel> UpdateCommentAsync(CommentModel comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

    }
}