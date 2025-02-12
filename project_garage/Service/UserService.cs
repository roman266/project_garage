using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using Microsoft.AspNetCore.Mvc;

namespace project_garage.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;

        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
            _emailSender = new EmailSender();
        }

        public async Task<IdentityResult> CreateUserAsync(string userName, string email, string password, string baseUrl)
        {
            var isUserExist = await CheckForExistanceByEmail(email);

            if (!isUserExist)
            {
                var user = new UserModel
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmationCode = Guid.NewGuid().ToString()
                };

                var result = await _userRepository.CreateUserAsync(user, password);

                if (result.Succeeded)
                {
                    Console.WriteLine("succed");
                    var confirmationLink = $"{baseUrl}confirmEmail?userId={user.Id}&code={user.EmailConfirmationCode}";

                    await _emailSender.SendEmailAsync(email, "Підтвердження email",
                        $"Перейдіть за посиланням для підтвердження акаунта: <a href='{confirmationLink}'>посилання</a>");
                    Console.WriteLine("sended");
                    return result;
                }

                throw new Exception("This username is used");
            }
            throw new Exception("User with this email already exist");
        }

        public async Task<bool> CheckForExistanceByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return false;
            return true;
        } 
        public async Task<UserModel> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new Exception("Wrong input data");

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new Exception("Wrong email");

            return user;
        }

        public async Task<UserModel> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new Exception("User not founded");

            return user;
        }

        public async Task<IdentityResult> UpdateUserInfoAsync(UserModel user)
        {
            var result = await _userRepository.UpdateUserInfoAsync(user);
            return result;
        }

        public async Task<IdentityResult> ConfirmUserEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                throw new Exception("User is null here");
            }

            var user = await GetByIdAsync(userId);
            if(user == null)
            {
                throw new Exception("User not founded");
            }
            if (user.EmailConfirmationCode != code)
                throw new Exception("Wrong code");

            user.EmailConfirmed = true;
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
