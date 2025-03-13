namespace project_garage.Models.ViewModels
{
    public class AuthDto
    {
        public string JwtToken {  get; set; }
        public CookieOptions CookieOptions { get; set; }
    }
}
