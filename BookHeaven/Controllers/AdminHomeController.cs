using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;
using BookHeaven.Extensions;


namespace BookHeaven.Controllers
{
    public class AdminHomeController : Controller
    {
        // this part is for the session variables
        private readonly IHttpContextAccessor _contx;

        public AdminHomeController(IHttpContextAccessor contx)
        {
            _contx = contx;
        }
        // # # #

        public IActionResult showAdminHome()
        {
            if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                CachingHelper.PreventCaching(HttpContext);

            ViewBag.GeneralMessage = TempData["GeneralMessage"] as string;
            return View("AdminHomeView", initHomeBooks());
        }

        public IActionResult userLogout()
        {
            Console.WriteLine(Models.User.currentUser.fname + " has logged out"); //print user name from the user object
            Models.User.currentUser = new User();
            _contx.HttpContext.Session.SetString("isLoggedIn", "false");
            return RedirectToAction("showUserHome", "UserHome");
        }

        private SearchResults initHomeBooks()
        {
            SearchResults searchResults = new SearchResults("");
            searchResults = SQLHelper.SQLSearchBook(searchResults);
            _contx.HttpContext.Session.SetObjectAsJson("listOfBooks", searchResults.books); // Store the initial list in session
            return searchResults;
        }

    }
}
