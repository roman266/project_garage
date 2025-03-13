using project_garage.Models.DbModels;

namespace project_garage.Interfaces.IService {
    public interface ICommentService
    {
        Task<CommentModel> AddCommentAsync(string postId, string userId, string content);
        Task<IEnumerable<CommentModel>> GetCommentsByPostIdAsync(string postId);
        Task<CommentModel> GetCommentByIdAsync(int commentId);
        Task<bool> DeleteCommentAsync(int commentId);
        Task<CommentModel> UpdateCommentAsync(int commentId, string userId, string content);

    }
}