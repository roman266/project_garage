using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;


namespace project_garage.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public AuthService(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        public async Task<AuthDto> SignInAsync(string email, string password)
        {
            var user = await _userService.GetByEmailAsync(email);

            if (user == null)
                throw new ArgumentException(nameof(user));

            var isPasswordValid = await _userService.CheckPasswordAsync(user, password);

            if (!isPasswordValid)
                throw new Exception("Invalid email or password");

            if (!user.EmailConfirmed)
                throw new Exception("You need to confirm your email");

            var token = _jwtService.GenerateToken(user.Id, user.Email);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            var authDto = new AuthDto
            {
                JwtToken = token,
                CookieOptions = cookieOptions,
            };

            return authDto; 
        }

        public async Task SignOutAsync()
        {
            var user = await _userService.GetByIdAsync("1");
            if (user == null)
                Console.WriteLine("Logout");
        }
    }
}
