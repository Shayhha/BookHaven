using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace BookHeaven.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult showAdminHome()
        {
            Login login = new Login();
            return View("AdminHomeView", login);
        }

       
    }
}
