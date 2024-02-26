using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class SearchResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
