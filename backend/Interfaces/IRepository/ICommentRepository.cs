using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IRepository {
    public interface ICommentRepository
    {
        Task<CommentModel> CreateCommentAsync(CommentModel comment);
        Task<CommentModel> GetCommentByIdAsync(int commentId);
        Task<IEnumerable<CommentModel>> GetCommentsByPostIdAsync(string postId);
        Task DeleteCommentAsync(CommentModel comment);
        Task<CommentModel> UpdateCommentAsync(CommentModel comment);

    }
}

