using BookHaven.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookHaven.Controllers
{
    public class UserHomeController : Controller
    {
        public IActionResult showUserHome()
        {
            Console.WriteLine(SQLHelper.ToSHA256("Shay1234"));
            return View("UserHomeView");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
