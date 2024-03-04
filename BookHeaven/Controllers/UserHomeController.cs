using BookHeaven.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class UserHomeController : Controller
    {
        public IActionResult showUserHome()
        {
            //Console.WriteLine(SQLHelper.ToSHA256("Shay1234"));

            SearchResults searchResults = new SearchResults("");
            searchResults = SQLHelper.SQLSearchBook(searchResults);

            return View("UserHomeView", searchResults);
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
                if (SQLHelper.SQLLogin(login) != "")
                {
                    // User found in the database
                    return View("UserHomeView");
                }
                else
                {
                    // User not found in the database
                    return View("LoginView", login);
                }
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
                if (SQLHelper.SQLSignup(signup) != "")
                {
                    return View("UserHomeView", signup);
                }
                else
                {
                    return View("SignupView", signup);
                }
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
