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

        public IActionResult tryToLogin(Login login)
        {
            if (ModelState.IsValid)
            {
                if (SQLHelper.SQLLogin(login) != "")
                {
                    // User found in the database
                    return View("../UserHome/UserHomeView");
                }
                else
                {
                    // User not found in the database
                    return View("AdminHomeView", login);
                }
            }
            else
            {
                return View("AdminHomeView", login);
            }
        }
    }
}
