using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using project_garage.Models.ViewModels;


namespace project_garage.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthService(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public async Task<AuthDto> SignInAsync(string email, string password)
        {
            var user = await _userService.GetByEmailAsync(email);

            if (user == null)
                throw new ArgumentException(nameof(user));

            var isPasswordValid = await _userService.CheckPasswordAsync(user, password);

            if (!isPasswordValid)
                throw new InvalidOperationException("Invalid email or password");

            if (!user.EmailConfirmed)
                throw new InvalidOperationException("You need to confirm your email");

            var authDto = await _tokenService.StartSessionAsync(user.Id, user.Email);

            return authDto; 
        }
    }
}
