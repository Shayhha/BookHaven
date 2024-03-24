using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;
using BookHeaven.Extensions;


namespace BookHeaven.Controllers
{
    public class UserHomeController : Controller
    {
        // this part is for the session variables
        private readonly IHttpContextAccessor _contx;

        public UserHomeController(IHttpContextAccessor contx)
        {
            _contx = contx;
        }
        // # # #


        public IActionResult showUserHome()
        {
            string message = TempData["GeneralMessage"] as string;
            ViewBag.GeneralMessage = message;
            return View("UserHomeView", initHomeBooks());
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
                User user = SQLHelper.SQLLogin(login); //try to login with user credentials, if succeed we get a user object
                if (user != null) // User found in the database
                {
                    User currentDefaltUser = Models.User.currentUser;
                    user.mergeCartLists(currentDefaltUser);

                    Models.User.currentUser = user; //set the user obj to be our static currentUser obj
                    Console.WriteLine(user.fname + " " + user.lname); //print user name from the user object
                    _contx.HttpContext.Session.SetString("isLoggedIn", "true"); //open new session for user
                    return View("UserHomeView", initHomeBooks());
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
                try
                {
                    User user = SQLHelper.SQLSignup(signup); //try to login with user credentials, if succeed we get a user object
                    if (user != null) // User found in the database
                    {
                        Models.User.currentUser = user; //set the user obj to be our static currentUser obj
                        Console.WriteLine(user.fname + " " + user.lname); //print user name from the user object
                        _contx.HttpContext.Session.SetString("isLoggedIn", "true"); //open new session for user
                        return View("UserHomeView", initHomeBooks());
                    }
                    else
                    {
                        return View("SignupView", signup);
                    }
                }
                catch
                {
                    Console.WriteLine("in catch block");
                    ViewBag.ShowEmailTakenMessage = true;
                    return View("SignupView", signup);
                }
            }
            else
            {
                return View("SignupView", signup);
            }
        }

        public IActionResult userLogout()
        {
            Console.WriteLine(Models.User.currentUser.fname + " has logged out"); //print user name from the user object
            Models.User.currentUser = new User();
            _contx.HttpContext.Session.SetString("isLoggedIn", "false");
            return RedirectToAction("showUserHome");
        }

        private SearchResults initHomeBooks()
        {
            SearchResults searchResults = new SearchResults("");
            searchResults = SQLHelper.SQLSearchBook(searchResults);
            _contx.HttpContext.Session.SetObjectAsJson("listOfBooks", searchResults.books); // Store the initial list in session
            return searchResults;
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
