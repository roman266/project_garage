using Microsoft.AspNetCore.Identity;
using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;

namespace project_garage.Service
{
    public class UserService : IUserService

    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public async Task<IdentityResult> CreateUserAsync(UserModel user, string password)
        {
            var result = await _userRepository.CreateUserAsync(user, password);

            return result;
        }

        public async Task<UserModel> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user;
        }

        public async Task<UserModel> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user;
        }

        public async Task<IdentityResult> UpdateUserInfoAsync(UserModel user)
        {
            var result = await _userRepository.UpdateUserInfoAsync(user);
            return result;
        }

        public async Task<IdentityResult> ConfirmUserEmail(UserModel user)
        {
            user.EmailConfirmed = true;
            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = "none";
            var result = await UpdateUserInfoAsync(user);
            return result;
        }

        public async Task<bool> CheckPasswordAsync(UserModel user, string password)
        {
            var result = await _userRepository.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var result = await _userRepository.DeleteUserAsync(id);
            return result;
        }

        public async Task<List<UserModel>> SearchUsersAsync(string query)
        {

            var users = await _userRepository.SearchUsersAsync(query);
            
            return users ?? new List<UserModel>();
        }
    }
}
