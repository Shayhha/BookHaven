using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
