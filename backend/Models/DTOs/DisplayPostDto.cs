using System.ComponentModel.DataAnnotations;

namespace project_garage.Models.DTOs
{
    public class DisplayPostDto
    {
        public string PostId { get; set; }
        public string PostDescription { get; set; }
        public string PostImageUrl { get; set; }
        public DateTime PostDate { get; set; }
        public string SenderAvatarUlr { get; set; }
        public string SenderNickName { get; set; }
    }
}
