namespace project_garage.Models.ViewModels
{
    public class EditPostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
