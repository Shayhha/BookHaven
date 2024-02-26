using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
