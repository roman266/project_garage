using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.ViewModels
{
    public class MessageOnCreationDto
    {
        [Required(ErrorMessage = "ConversationId is required")]
        public string ConversationId { get; set; }

        [Required(ErrorMessage = "SenderId is required")]
        public string SenderId { get; set; }

        [Required(ErrorMessage = "Message text is required")]
        [StringLength(1000, ErrorMessage = "Message text cannot exceed 1000 characters")]
        public string Text { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Text) || Text.Length > 1000 || string.IsNullOrEmpty(ConversationId) || string.IsNullOrEmpty(SenderId))
            {
                throw new ArgumentException("Invalid data. Please check the fields.");
            }
        }
    }
}
