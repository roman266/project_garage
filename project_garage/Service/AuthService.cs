using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;


namespace project_garage.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authService;

        public AuthService(IAuthRepository authService)
        {
            _authService = authService;
        }

        public async Task SignInAsync(UserModel userModel)
        {
            await _authService.SignInAsync(userModel);
        }

        public async Task SignOutAsync()
        {
            await _authService.SignOutAsync();
        }
    }
}
