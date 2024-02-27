using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;

namespace BookHeaven.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult showAdminHome()
        {
            Login login = new Login(); 
            return View("AdminHomeView", login);
        }

        public IActionResult tryToLogin(Login login)
        {
            if (ModelState.IsValid)
            {
                // add some sql stuff here
                return View("../UserHome/UserHomeView");
            }
            else
            {
                return View("AdminHomeView", login);
            }
        }
    }
}
