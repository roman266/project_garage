using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
            throw new Exception("User with this email already exists");
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

        public async Task<IdentityResult> UpdateUserInfoAsync(string userId, string userName, string firstName, string lastName, string description, string email, string password)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Wrong userId");

            var user = await _userRepository.GetByIdAsync(userId);

            if (!string.IsNullOrEmpty(userName))
                user.UserName = userName;

            if (!string.IsNullOrEmpty(firstName))
                user.FirstName = firstName;

            if (!string.IsNullOrEmpty(lastName))
                user.LastName = lastName;

            if (!string.IsNullOrEmpty(description))
                user.Description = description;

            if (!string.IsNullOrEmpty(email))
                user.Email = email;

            if (!string.IsNullOrEmpty(password))
            {
                var result = await _userRepository.UpdateUserPasswordAsync(user.Id, password);
                if (!result.Succeeded)
                    return result;
            }

            return await _userRepository.UpdateUserInfoAsync(user);
        }

        public async Task<IdentityResult> UpdateProfilePictureAsync(string userId, string picture)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(picture))
                throw new ArgumentException("Wrong input data");

            var user = await GetByIdAsync(userId);

            user.ProfilePicture = picture;

            var result = await _userRepository.UpdateUserInfoAsync(user);

            return result;
        }

        public async Task ConfirmUserEmail(string userId, string code)
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
            
            return users ?? new List<UserModel>();
        }
    }
}
