using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class SignupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
