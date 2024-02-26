using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult showAdminHome()
        {
            return View("AdminHomeView");
        }
    }
}
