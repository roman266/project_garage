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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid post data");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var post = new PostModel
            {
                Title = model.Title,
                Description = model.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _postService.CreatePostAsync(post);

            return RedirectToAction("ProfileIndex", new { userId = userId });
        }
        [HttpGet]
        [Route("Profile/EditPost/{postId}")]
        public async Task<IActionResult> EditPost(Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var model = new EditPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Profile/EditPost")]
        public async Task<IActionResult> EditPostSave(EditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var post = await _postService.GetPostByIdAsync(model.Id);
                post.Title = model.Title;
                post.Description = model.Description;
                post.UpdatedAt = DateTime.UtcNow;

                await _postService.UpdatePostAsync(post);
                return RedirectToAction("ProfileIndex", new { userId = post.UserId });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error occurred while updating the post.");
            }
        }

        [HttpPost]
        [Route("Profile/DeletePost")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(postId);
                if (post == null)
                {
                    return NotFound();
                }

                await _postService.DeletePostAsync(postId);
                return RedirectToAction("ProfileIndex", new { userId = post.UserId });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error occurred while deleting the post.");
            }
        }


    }
}
