using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;

namespace project_garage.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly string _baseUrl;

        public UserService(IUserRepository userRepository, IConfiguration config) 
        {
            _userRepository = userRepository;
            _emailSender = new EmailSender();
            _baseUrl = config["MailSender:FrontendURL"];
        }

        public async Task<IdentityResult> CreateUserAsync(string userName, string email, string password)
        {
            
            var isUserExist = await CheckForExistanceByEmail(email);
            

            if (isUserExist)
                throw new InvalidOperationException("User with this email already exists");

            var userModel = new UserModel
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmationCode = Guid.NewGuid().ToString()
                };

            var result = await _userRepository.CreateUserAsync(userModel, password);
            if (!result.Succeeded)
                throw new ArgumentException("This username is already in use");

            await SendEmailAsync(email);

            return result;
        }

        public async Task SendEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            var confirmationLink = $"{_baseUrl}/confirmEmail?userId={user.Id}&code={user.EmailConfirmationCode}";
            await _emailSender.SendEmailAsync(email, "Підтвердження email",
                $"Перейдіть за посиланням для підтвердження акаунта: <a href='{confirmationLink}'>посилання</a>");
        }

        public async Task<bool> CheckForExistanceByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            
            return user != null;
        } 
        public async Task<UserModel> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Wrong input data");

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new KeyNotFoundException($"User with email {email} does not exist");

            return user;
        }

        public async Task<UserModel> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("User not founded");

            return user;
        }

        public async Task<IdentityResult> UpdateUserInfoAsync(string userId, string firstName, string lastName, string description)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Invalid userId");

            var user = await GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

            user.FirstName = string.IsNullOrWhiteSpace(firstName) ? user.FirstName : firstName;
            user.LastName = string.IsNullOrWhiteSpace(lastName) ? user.LastName : lastName;
            user.Description = string.IsNullOrWhiteSpace(description) ? user.Description : description;

            return await _userRepository.UpdateUserInfoAsync(user);
        }

        public async Task<IdentityResult> UpdateProfilePictureAsync(string userId, string picture)
        {
            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(picture))
                throw new ArgumentException("Wrong input data");

            var user = await GetByIdAsync(userId);
            user.ProfilePicture = picture;
            var result = await _userRepository.UpdateUserInfoAsync(user);

            return result;
        }

        public async Task ConfirmUserEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                throw new ArgumentException("User is null here");

            var user = await GetByIdAsync(userId);

            if (user.EmailConfirmationCode != code)
                throw new ArgumentException("Wrong code");

            user.EmailConfirmed = true;
            user.EmailConfirmationCode = "none";
            await _userRepository.UpdateUserInfoAsync(user);
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
            return users;
        }

        public async Task<bool> CheckForExistanceByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null;
        }
    }
}
