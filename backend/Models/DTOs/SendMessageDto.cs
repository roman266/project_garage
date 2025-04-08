using System;
using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.DTOs
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "MessageId is required.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "ConversationId is required.")]
        public string ConversationId { get; set; }

        [Required(ErrorMessage = "SenderId is required.")]
        public string SenderId { get; set; }

        [Required(ErrorMessage = "Text is required.")]
        [StringLength(1000, ErrorMessage = "Text cannot be longer than 1000 characters.")]
        public string Text { get; set; }

        [Required(ErrorMessage = "ImageUrl must be a valid URL.")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "SendedAt is required.")]
        public DateTime SendedAt { get; set; }

        [Required(ErrorMessage = "IsEdited is required.")]
        public bool IsEdited { get; set; }

        [Required(ErrorMessage = "IsReaden is required.")]
        public bool IsReaden { get; set; }

        [Required(ErrorMessage = "IsVisible is required.")]
        public bool IsVisible { get; set; }
    }
}
