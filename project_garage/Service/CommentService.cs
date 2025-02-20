using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Service {
     public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<CommentModel> AddCommentAsync(Guid postId, string userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content cannot be empty.");
            }

            var comment = new CommentModel
            {
                PostId = postId,
                UserId = userId,
                Content = content
            };

            return await _commentRepository.CreateCommentAsync(comment);
        }

        public async Task<IEnumerable<CommentModel>> GetCommentsByPostIdAsync(Guid postId)
        {
           return await _commentRepository.GetCommentsByPostIdAsync(postId);
        }

       public async Task<CommentModel> GetCommentByIdAsync(int commentId)
        {
            return await _commentRepository.GetCommentByIdAsync(commentId);
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                return false; 
            }

            await _commentRepository.DeleteCommentAsync(comment);
            return true;
        }

        public async Task<CommentModel> UpdateCommentAsync(int commentId, string userId, string content)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("Ви не маєте прав редагувати цей коментар.");
            }

            comment.Content = content;
            return await _commentRepository.UpdateCommentAsync(comment);
        }

    }
}