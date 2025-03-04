using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService {
    public interface ICommentService
    {
        Task<CommentModel> AddCommentAsync(Guid postId, string userId, string content);
        Task<IEnumerable<CommentModel>> GetCommentsByPostIdAsync(Guid postId);
        Task<CommentModel> GetCommentByIdAsync(int commentId);
        Task<bool> DeleteCommentAsync(int commentId);
        Task<CommentModel> UpdateCommentAsync(int commentId, string userId, string content);

    }
}