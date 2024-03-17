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
            SearchResults searchResults = new SearchResults(searchQuery, 1);
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


        public IActionResult filterBooks(string filterBy, string searchQuery = "", int isCategory = 0)
        {
            SearchResults searchResults;

            if (searchQuery == "")
                searchResults = new SearchResults("All Books");
            else
                searchResults = new SearchResults(searchQuery);

            // Get the books that we have saved in the session variable
            List<Book> bookList = _contx.HttpContext.Session.GetObjectFromJson<List<Book>>("listOfBooks");

            // Call a function that will filter the sessionBooks
            bookList = filterTheBooks(bookList, filterBy, searchResults.searchQuery, isCategory);

            // Then we add the books in the correct order to the searchResults object and send it to the users screen
            foreach (Book book in bookList)
            {
                searchResults.books.Add(book);
            }

            ViewBag.SelectedFilter = filterBy;
            return View("SearchResultsView", searchResults);
        }

        public List<Book> filterTheBooks(List<Book> bookList, string filterBy, string searchQuery = "", int isCategory = 0) // temporary function, will be replaced by other filter functions
        {
            if (filterBy == "A - Z")
                return bookList.OrderBy(book => book.name).ToList();
            else if (filterBy == "Z - A")
                return bookList.OrderByDescending(book => book.name).ToList();
            else if (filterBy == "Price Ascending")
                return bookList.OrderBy(book => book.salePrice > 0 ? book.salePrice : book.price).ToList();
            else if (filterBy == "Price Descending")
                return bookList.OrderByDescending(book => book.salePrice > 0 ? book.salePrice : book.price).ToList();
            else if (filterBy == "Most Popular")
            {
                if (isCategory == 0)
                {
                    if (searchQuery == "All Books")
                        searchQuery = "";
                    return SQLHelper.SQLSearchPopularBook(searchQuery);
                }
                else
                    return SQLHelper.SQLSearchPopularCategory(searchQuery);
            }
            else
                return bookList;
        }
    }
}
