
namespace project_garage.Models.DbModels {
public class CommentModel
    {
        public int Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public PostModel Post { get; set; }
        public UserModel User { get; set; }
    }
}