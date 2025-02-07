using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;


namespace project_garage.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;

        public AuthService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserModel> SignInAsync(string email, string password)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("No user founded");

            var isPasswordValid = await _userService.CheckPasswordAsync(user, password);

            if (!isPasswordValid)
                throw new Exception("Invalid email or password");
            if (user.EmailConfirmed)
                    return user;

            throw new Exception("You need to confirm your email");
        }

        public async Task SignOutAsync()
        {
            var user = await _userService.GetByIdAsync("1");
            if (user == null)
                Console.WriteLine("Logout");
        }
    }
}
