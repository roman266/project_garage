using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace project_garage.Models.DbModels
{
    public class UserModel : IdentityUser
    {
        public string ProfilePicture { get; set; } = "None";
        [Required]
        public string Role { get; set; } = "User"; // Default role

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Automatically set current time

        public DateTime? UpdatedAt { get; set; }

        public string Description { get; set; } = "None"; // Optional description of user

        public DateTime? LastLogin { get; set; } // Nullable, updated when the user logs in

        public string ActiveStatus { get; set; } = "Online"; // Status like "Online", "Away", etc.

        public string AccountStatus { get; set; } = "Active"; // Account status: "Active", "Banned", etc.
        public bool IsEmailConfirmed { get; set; }
        public string EmailConfirmationCode { get; set; }
        public ICollection<PostModel> Posts { get; set; }
        public ICollection<FriendModel> Friends { get; set; }
    }
}

