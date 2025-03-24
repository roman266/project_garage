using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace project_garage.Data
{
    public static class UserHelper
    {
        public static string GetCurrentUserId(HttpContext httpContext)
        {
            var user = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(user))
                throw new UnauthorizedAccessException("Unauthorized");
            
            return user;
        }
    }
}
