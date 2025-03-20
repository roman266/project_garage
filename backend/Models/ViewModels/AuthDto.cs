namespace project_garage.Models.ViewModels
{
    public class AuthDto
    {
        public string AccessToken {  get; set; }
        public string RefreshToken { get; set; }
        public CookieOptions AccessCookieOptions { get; set; }
        public CookieOptions RefreshCookieOptions { get; set; }
    }
}
