using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace project_garage.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Profile(string id)
        {
            // Логіка отримання даних профілю користувача
            return View();
        }
    }
}
