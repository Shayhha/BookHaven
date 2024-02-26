using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
