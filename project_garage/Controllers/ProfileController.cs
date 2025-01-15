using Microsoft.AspNetCore.Mvc;
using project_garage.Service;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IRepository;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;

namespace project_garage.Controllers
{

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostService _postService;
        private readonly IFriendService _friendService;

        public ProfileController(IUserRepository userRepository, IFriendService friendService, IPostService postService)
        {
            _userRepository = userRepository;
            _friendService = friendService;
            _postService = postService;
        }

        [Route("Profile/ProfileIndex/{userId}")]
        public async Task<IActionResult> ProfileIndex(string userId)
        {
            Console.WriteLine($"USER ID: {userId}");
            var user = await _userRepository.GetByIdAsync(userId);
   
         
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
    }
}
