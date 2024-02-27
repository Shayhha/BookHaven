using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
