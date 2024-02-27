using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View("OrdersView");
        }
    }
}
