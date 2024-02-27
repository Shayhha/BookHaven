using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View("OrdersView");
        }
    }
}
