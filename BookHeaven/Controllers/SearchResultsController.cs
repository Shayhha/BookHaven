using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;
using System.Text.RegularExpressions;
using BookHeaven.Extensions;

namespace BookHeaven.Controllers
{
    public class SearchResultsController : Controller
    {
        // this part is for the session variables
        private readonly IHttpContextAccessor _contx;

        public SearchResultsController(IHttpContextAccessor contx)
        {
            _contx = contx;
        }
        // # # #


        public IActionResult showSearchResults(string searchQuery)
        {
            if (searchQuery == null) return StatusCode(204); // tells the browser that the request was successful but there is no content to display.

            SearchResults searchResults = new SearchResults(searchQuery);

            if (isSearchQueryBookId(searchQuery))
                searchResults = SQLHelper.SQLSearchBook(searchResults, false);
            else
                searchResults = SQLHelper.SQLSearchBook(searchResults);

            _contx.HttpContext.Session.SetObjectAsJson("listOfBooks", searchResults.books); // Store the book list in session

            return View("SearchResultsView", searchResults);
        }

        public IActionResult showCategoryResults(string searchQuery)
        {
            SearchResults searchResults = new SearchResults(searchQuery);
            searchResults = SQLHelper.SQLSearchCategory(searchResults);

            _contx.HttpContext.Session.SetObjectAsJson("listOfBooks", searchResults.books); // Store the book list in session

            return View("SearchResultsView", searchResults);
        }

        private bool isSearchQueryBookId(string searchQuery)
        {
            string pattern = "^[0-9]+$";
            return Regex.IsMatch(searchQuery, pattern);
        }

        public IActionResult showBookDetails(int bookId)
        {
            return RedirectToAction("showBookEdit", "Book", new { bookId = bookId });
        }




        public IActionResult filterBooks(string filterBy, string searchQuery = "")
        {
            SearchResults searchResults;

            if (searchQuery == "")
                searchResults = new SearchResults("All Books");
            else
                searchResults = new SearchResults(searchQuery);

            // Get the books that we have saved in the session variable
            List<Book> listOfBooks = _contx.HttpContext.Session.GetObjectFromJson<List<Book>>("listOfBooks");

            // Call a function that will filter the sessionBooks
            listOfBooks = filterTheBooks(listOfBooks);

            // Then we add the books in the correct order to the searchResults object and send it to the users screen
            foreach (Book book in listOfBooks)
            {
                searchResults.books.Add(book);
            }

            ViewBag.SelectedFilter = filterBy;
            return View("SearchResultsView", searchResults);
        }

        public List<Book> filterTheBooks(List<Book> listOfBooks) // temporary function, will be replaced by other filter functions
        {
            return listOfBooks;
        }
    }
}
