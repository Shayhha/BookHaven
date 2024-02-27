using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class SearchResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
