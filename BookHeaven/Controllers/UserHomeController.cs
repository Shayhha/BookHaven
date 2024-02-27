using BookHeaven.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class UserHomeController : Controller
    {
        public IActionResult showUserHome()
        {
            return View("UserHomeView");
        }

        public IActionResult showLoginView()
        {
            Login login = new Login();
            return View("LoginView", login);
        }

        public IActionResult showSignupView()
        {
            Signup signup = new Signup();
            return View("SignupView", signup);
        }


        public IActionResult tryToLogin(Login login)
        {
            if (ModelState.IsValid)
            {
                // add some SQL stuff here
                return View("UserHomeView", login);
            }
            else
            {
                return View("LoginView", login);
            }
        }

        public IActionResult tryToSignup(Signup signup)
        {
            if (ModelState.IsValid)
            {
                // add some SQL stuff here
                return View("UserHomeView", signup);
            }
            else
            {
                return View("SignupView", signup);
            }
        }



        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
