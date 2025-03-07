using System.Data;

namespace project_garage.Models.DbModels
{
    public class PostModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserModel User { get; set; }
        public ICollection<CommentModel> Comments { get; set; }
        public ICollection<PostImageModel> Images { get; set; } = new List<PostImageModel>();
    }
}
