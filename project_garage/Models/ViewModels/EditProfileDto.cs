namespace project_garage.Models.ViewModels
{
    public class EditProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; } // Добавляем поле Email
        public string Password { get; set; } // Добавляем поле Password
    }
}
