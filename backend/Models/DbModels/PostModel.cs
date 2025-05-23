﻿using System.Data;

namespace project_garage.Models.DbModels
{
    public class PostModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserModel User { get; set; }
        public int InterestId { get; set; }

        public InterestModel Interest { get; set; }
        public ICollection<CommentModel> Comments { get; set; }
        public ICollection<PostImageModel> Images { get; set; }
    }
}
