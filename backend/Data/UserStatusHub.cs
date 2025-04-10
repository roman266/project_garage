using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using project_garage.Interfaces.IService;

public class UserStatusHub : Hub
{
    private readonly IUserService _userService;

    public UserStatusHub(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await _userService.UpdateUserStatusAsync(userId, "Online");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await _userService.UpdateUserStatusAsync(userId, "Offline");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
