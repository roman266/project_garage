using Microsoft.AspNetCore.Identity;
using project_garage.Interfaces.IRepository;
using project_garage.Models.DbModels;

namespace project_garage.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly SignInManager<UserModel> _signInManager;
        public AuthRepository(SignInManager<UserModel> signInManager) 
        {
            _signInManager = signInManager;
        }

        public async Task SignInAsync(UserModel userModel)
        {
            await _signInManager.SignInAsync(userModel, isPersistent: true);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
