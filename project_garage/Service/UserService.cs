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

        public async Task CreateUserAsync(string userName, string email, string password, string baseUrl)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(baseUrl))
                throw new InvalidDataException("Invalida data");

            if (!await CheckForExistanceByEmail(email))
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
                    Console.WriteLine($"succed, {user.Id}");
                    var confirmationLink = $"{baseUrl}/Account/ConfirmEmail?userId={user.Id}&code={user.EmailConfirmationCode}";

                    await _emailSender.SendEmailAsync(email, "Підтвердження email",
                        $"Перейдіть за посиланням для підтвердження акаунта: <a href='{confirmationLink}'>посилання</a>");
                    Console.WriteLine("sended");
                }
                throw new Exception("User with this username already exist");
            }

            throw new Exception("User with this email already exist");
        }

        public async Task<bool> CheckForExistanceByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new InvalidDataException("Invalida data");

            var user = await GetByEmailAsync(email);
            if (user == null)
                return false;
            return true;
        }
           
 
        public async Task<UserModel> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new InvalidDataException("Invalida data");

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new Exception("User not founded");

            return user;
        }

        public async Task<UserModel> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new InvalidDataException("Invalida data");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not founded");
            return user;
        }

        public async Task UpdateUserInfoAsync(UserModel user)
        {
            if (user == null)
                throw new InvalidDataException();

            var result = await _userRepository.UpdateUserInfoAsync(user);
        }

        public async Task<IdentityResult> ConfirmUserEmail(UserModel user)
        {
            user.EmailConfirmed = true;
            user.EmailConfirmationCode = "none";
            await UpdateUserInfoAsync(user);
            return IdentityResult.Success;
        }

        public async Task<bool> CheckPasswordAsync(UserModel user, string password)
        {
            var result = await _userRepository.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task DeleteUserAsync(string id)
        {
            var result = await _userRepository.DeleteUserAsync(id);
        }

        public async Task<List<UserModel>> SearchUsersAsync(string query)
        {

            var users = await _userRepository.SearchUsersAsync(query);
            
            return users ?? new List<UserModel>();
        }
    }
}
