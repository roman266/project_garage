using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;


namespace project_garage.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserService _userService;

        public AuthService(IAuthRepository authRepository, IUserService userService)
        {
            _authRepository = authRepository;
            _userService = userService;
        }

        public async Task SignInAsync(string email, string password)
        {
            var userModel = await _userService.GetByEmailAsync(email);
            if (userModel == null)
                throw new Exception("No user founded");


            if (await _userService.CheckPasswordAsync(userModel, password))
            {
                if (userModel.IsEmailConfirmed)
                {
                    await _authRepository.SignInAsync(userModel);
                }
                else
                {
                    throw new Exception("Email isn't confirmed");
                }
            }
            else 
            { 
                throw new Exception("Wrong password"); 
            }
        }

        public async Task SignOutAsync()
        {
            await _authRepository.SignOutAsync();
        }
    }
}
