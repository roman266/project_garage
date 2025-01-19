﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using project_garage.Data;
using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using project_garage.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using System.Web;

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
            if (!await CheckForExistanceByEmail(email))
            {
                var user = new UserModel
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmationCode = Guid.NewGuid().ToString()
                };

                var result = await _userRepository.CreateUserAsync(user, password);
                var emailSend = await _userRepository.GetByEmailAsync(email);

                if (result.Succeeded)
                {
                    Console.WriteLine($"succed, {user.Id}");
                    var confirmationLink = $"{baseUrl}/Account/EmailConfirmed/{emailSend.Id}?code={emailSend.EmailConfirmationCode}";
                    await _emailSender.SendEmailAsync(email, "Підтвердження email",
                        $"Перейдіть за посиланням для підтвердження акаунта: <a href='{confirmationLink}'>посилання</a>");
                    Console.WriteLine("sended");
                    return result;
                }
            }
            throw new Exception("User with this email already exist");
        }

        public async Task<bool> CheckForExistanceByEmail(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
                return false;
            return true;
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
