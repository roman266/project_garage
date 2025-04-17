using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.DTOs
{
    public class MessageReceiveNotificationDto
    {
        [Required]
        public string SenderUserName { get; set; }
        [Required]
        public string SenderProfilePicture { get; set; }
    }
}
