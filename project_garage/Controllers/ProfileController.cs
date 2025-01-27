using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using project_garage.Models.DbModels;
using project_garage.Data;
using System.Security.Claims;

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
            var loggedInUserId = User.GetUserId(); // Отримуємо ID залогіненого користувача

            Console.WriteLine($"USER ID: {userId}, LOGGED IN USER ID: {loggedInUserId}");
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var canAddFriend = await _friendService.CanAddFriendAsync(loggedInUserId, userId);

            // Отримання постів користувача
            var userPosts = await _postService.GetPostsByUserIdAsync(userId);

            var viewModel = new ProfileViewModel
            {
                UserId = user.Id,
                Nickname = user.UserName,
                Description = user.Description,
                FriendsCount = await _friendService.GetFriendsCount(userId),
                PostsCount = await _postService.GetCountOfPosts(userId),
                CanAddFriend = canAddFriend,
                Posts = userPosts // Передаємо список постів у ViewModel
            };

            return View(viewModel);
        }


        [HttpPost]
        [Route("Friends/Add")]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var userId = User.GetUserId();

            try
            {
                await _friendService.SendFriendRequestAsync(friendId, userId);
                return Ok(new { message = "Friend request set successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred while sending the friend request." });
            }

        }

        [HttpPost]
        [Route("Profile/Accept")]
        public async Task<IActionResult> AcceptFriend(string friendId)
        {
            Console.WriteLine($"{friendId}");

            var userId = User.GetUserId();
            try
            {

                await _friendService.AcceptRequestAsync(userId, friendId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occured while accepting request" });
            }

            return View();
        }

        [HttpPost]
        [Route("Profile/Reject")]
        public async Task<IActionResult> RejectFriend(string friendId)
        {
            var userId = User.GetUserId();
            try
            {
                await _friendService.RejectOrDeleteAsync(userId, friendId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            { 
                return StatusCode(500, new { error = "An error occured while rejecting" }); 
            }
            return View();
        }

        [HttpGet]
        [Route("Profile/GetAllRequests")]
        public async Task<IActionResult> GetAllRequests()
        {
            var userId = User.GetUserId();
            Console.WriteLine("-------------------------");
            try
            {
                var requests = await _friendService.GetByUserIdAsync(userId);

                var viewModel = new List<FriendRequestViewModel>();

                foreach (var request in requests)
                {
                    if (!request.IsAccepted)
                    {
                        // Отримуємо дані друга через FriendId
                        var friend = await _userService.GetByIdAsync(request.FriendId);

                        if (friend != null)
                        {
                            viewModel.Add(new FriendRequestViewModel
                            {
                                RequestId = request.Id,
                                SenderId = friend.Id, // ID друга (відправника заявки)
                                SenderName = friend.UserName, // Ім'я друга
                                SenderDescription = friend.Description // Опис друга
                            });
                        }
                    }

                    return View(viewModel); // Передаємо список у View
                }

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occured while rejecting" });
            }
            return View();
        }

        [HttpPost]
        public IActionResult GetAllRequests(FriendRequestViewModel model)
        {
            return View(model);
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
