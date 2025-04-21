using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using project_garage.Models.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace project_garage.Models.DbModels
{
    public class UserModel : IdentityUser
    {
        public string ProfilePicture { get; set; } = "None";
        [Required]
        public string Role { get; set; } = "User"; // Default role
        public string FirstName { get; set; } = "None";
        public string LastName { get; set; } = "None";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatically set current time
        public DateTime? UpdatedAt { get; set; }
        public string Description { get; set; } = "None"; // Optional description of user
        public DateTime? LastLogin { get; set; } // Nullable, updated when the user logs in
        public string ActiveStatus { get; set; } = "Offline"; // Status like "Online", "Away", etc.
        public string AccountStatus { get; set; } = "Active"; // Account status: "Active", "Banned", etc.
        public string EmailConfirmationCode { get; set; }
        public ICollection<PostModel> Posts { get; set; }
        public ICollection<FriendModel> Friends { get; set; }
        public ICollection<CommentModel> Comments { get; set; }
        public ICollection<PostImageModel> Images { get; set; }
        public ICollection<UserConversationModel> UserConversations { get; set; }
        public ICollection<UserInterestModel> UserInterests { get; set; }
        public ICollection<RefreshTokenModel> RefreshTokens { get; set; }


    }
}

