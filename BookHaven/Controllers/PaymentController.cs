using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
