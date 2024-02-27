using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
