using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
