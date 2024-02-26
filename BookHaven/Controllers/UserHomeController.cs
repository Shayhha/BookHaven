using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class UserHomeController : Controller
    {
        public IActionResult showUserHome()
        {
            return View("UserHomeView");
        }
    }
}
