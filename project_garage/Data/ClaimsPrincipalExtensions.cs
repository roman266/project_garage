using System.Security.Claims;

namespace project_garage.Data
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
