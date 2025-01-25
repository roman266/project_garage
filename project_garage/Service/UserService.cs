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

        public async Task UpdateUserInfoAsync(string id, string userName, string description)
        {
            if (string.IsNullOrEmpty(id) && (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(description)))
                throw new InvalidDataException("Invalid data");

            var userByName = await _userRepository.FindByNameAsync(userName);

            if (userByName != null)
                throw new InvalidOperationException("This name is already used");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new InvalidDataException("Wrong user Id");

            if (description != null)
                user.Description = description;
            if (userByName != null)
                await _userRepository.ChangeName(user, userName);

            await _userRepository.UpdateUserInfoAsync(user);
        }

        public async Task ConfirmUserEmail(string id, string code)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(code))
                throw new InvalidDataException("Invalid data");
            
            var user = await GetByIdAsync(id);

            if (user == null)
                throw new InvalidOperationException("User not founded");

            if (user.EmailConfirmationCode != code )
                throw new InvalidOperationException("Worng confirmation code");

            if (user.EmailConfirmed)
                throw new InvalidOperationException("Email already confirmed");

            user.EmailConfirmed = true;
            user.EmailConfirmationCode = "none";
            await _userRepository.UpdateUserInfoAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(string id, string password)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
                throw new InvalidDataException("Invalid input  data");

            var user = await GetByIdAsync(id);

            if (user == null)
                throw new InvalidOperationException($"User not found");

            var result = await _userRepository.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task DeleteUserAsync(string id)
        {
            if (!string.IsNullOrEmpty(id)) 
                throw new InvalidDataException($"Invalid input data");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new InvalidOperationException($"User not found");

            await _userRepository.DeleteUserAsync(user);
        }

        public async Task<List<UserModel>> SearchUsersAsync(string query)
        {
            if (!string.IsNullOrEmpty(query))
                throw new InvalidDataException("Query is empty");

            var users = await _userRepository.SearchByQueryAsync(query);

            if (!users.Any())
                throw new InvalidOperationException($"User not found");

            return users;
        }
    }
}
