﻿using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using project_garage.Models.DbModels;

namespace project_garage.Controllers
{

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly IFriendService _friendService;

        public ProfileController(IUserService userRepository, IFriendService friendService, IPostService postService)
        {
            _userService = userRepository;
            _friendService = friendService;
            _postService = postService;
        }

        [Route("Profile/ProfileIndex/{userId}")]
        public async Task<IActionResult> ProfileIndex(string userId)
        {
            Console.WriteLine($"USER ID: {userId}");
            var user = await _userService.GetByIdAsync(userId);
   
         
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ProfileViewModel
            {
                Nickname = user.UserName,
                Description = user.Description,
                FriendsCount = await _friendService.GetFriendsCount(userId),
                PostsCount = await _postService.GetCountOfPosts(userId)
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult SearchUsers()
        {
            return View();
        }

        [HttpPost]
        [Route("Profile/SearchUsers")]
        public async Task<IActionResult> SearchUsers(SearchBoxViewModel model)
        {
            Console.WriteLine("IVE BEEN HERE----------------");
            Console.WriteLine($"{model.Query} ---------------");
            if (!string.IsNullOrEmpty(model.Query))
            {
                var users = await _userService.SearchUsersAsync(model.Query);
                Console.WriteLine($"{users.Count} USERS COUNT ------------------");
                return PartialView("_UserList", users);
            }

            return PartialView("_UserList", new List<UserModel>());
        }
    }
}
