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

        private IActionResult JsonResponse(object data, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            return Json(data);
        }

        [HttpPost]
        [Route("Friend/Send/{friendId}")]
        public async Task<IActionResult> SendRequest(string friendId)
        {
            try
            {
                var userId = User.GetUserId();
                Console.WriteLine(userId);
                await _friendService.SendFriendRequestAsync(friendId, userId);
                return JsonResponse(new { success = true, message = "Request sended" });
            }
            catch (InvalidDataException ex)
            {
                return JsonResponse(new {success = false, message = ex.Message}, 400);
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpPost]
        [Route("Friend/Accept/{friendId}")]
        public async Task<IActionResult> AcceptFriend(string friendId)
        {
            try
            {
                var userId = User.GetUserId();
                await _friendService.AcceptRequestAsync(userId, friendId);
                return JsonResponse(new { success = true, message = "Request accepted"});
            }
            catch (InvalidDataException ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 400);
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpPost]
        [Route("Friend/Reject/{friendId}")]
        public async Task<IActionResult> RejectFriend(string friendId)
        {
            try
            {
                var userId = User.GetUserId();
                await _friendService.RejectOrDeleteAsync(userId, friendId);
                return JsonResponse(new { success = true, message = "Request rejected" });
            }
            catch (InvalidDataException ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 400);
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpGet]
        [Route("Friend/GetRequests")]
        public async Task<IActionResult> GetUnacceptedRequests()
        {
            try
            {
                var userId = User.GetUserId();

                var requests = await _friendService.GetByUserIdAsync(userId);

                var viewModel = new List<FriendRequestViewModel>();

                foreach (var request in requests)
                {
                    if (!request.IsAccepted)
                    {
                        var friend = await _userService.GetByIdAsync(request.FriendId);
                        viewModel.Add(new FriendRequestViewModel
                        {
                            RequestId = request.Id,
                            SenderId = friend.Id,
                            SenderName = friend.UserName,
                            SenderDescription = friend.Description
                        });
                    }
                }
                return JsonResponse(new { success = true, message = "Request rejected" , friendList = viewModel});
            }
            catch (InvalidDataException ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 400);
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpGet]
        [Route("Friend/GetFriends")]
        public async Task<IActionResult> GetAcceptedRequests()
        {
            try
            {
                var userId = User.GetUserId();
                var requests = await _friendService.GetByUserIdAsync(userId);
                var viewModel = new List<FriendRequestViewModel>();

                foreach (var request in requests)
                {
                    if (request.IsAccepted)
                    {
                        var friend = await _userService.GetByIdAsync(request.FriendId);
                        viewModel.Add(new FriendRequestViewModel
                        {
                            RequestId = request.Id,
                            SenderId = friend.Id, // ID друга (відправника заявки)
                            SenderName = friend.UserName, // Ім'я друга
                            SenderDescription = friend.Description // Опис друга
                        });
                    }
                }
                return JsonResponse(new { success = true, message = "Request rejected", friendList = viewModel });
            }
            catch (InvalidDataException ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 400);
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

    }
}
