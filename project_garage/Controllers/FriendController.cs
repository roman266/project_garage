using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;
using project_garage.Models.ViewModels;
using project_garage.Service;

namespace project_garage.Controllers
{
    [Authorize]
    public class FriendController : Controller
    {
        private readonly IFriendService _friendService;
        private readonly IUserService _userService;

        public FriendController(IFriendService friendService, IUserService userService)
        {
            _friendService = friendService;
            _userService = userService;
        }

        [HttpPost]
        [Route("Friend/Add")]
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
        [Route("Friend/Accept")]
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
        [Route("Friend/Reject")]
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
        [Route("Friend/GetUnacceptedRequests")]
        public async Task<IActionResult> GetUnacceptedRequests()
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

        [HttpGet]
        [Route("Friend/GetAcceptedRequests")]
        public async Task<IActionResult> GetAcceptedRequests()
        {
            var userId = User.GetUserId();
            Console.WriteLine("-------------------------");
            try
            {
                var requests = await _friendService.GetByUserIdAsync(userId);

                var viewModel = new List<FriendRequestViewModel>();

                foreach (var request in requests)
                {
                    if (request.IsAccepted)
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

    }
}
