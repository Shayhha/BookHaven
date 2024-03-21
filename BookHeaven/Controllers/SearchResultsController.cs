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
                searchResults = new SearchResults(searchQuery, isCategory);

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
            switch (filterBy)
            {
                case "A - Z":
                    return bookList.OrderBy(book => book.name).ToList();
                case "Z - A":
                    return bookList.OrderByDescending(book => book.name).ToList();
                case "Price Ascending":
                    return bookList.OrderBy(book => book.salePrice > 0 ? book.salePrice : book.price).ToList();
                case "Price Descending":
                    return bookList.OrderByDescending(book => book.salePrice > 0 ? book.salePrice : book.price).ToList();
                case "Most Popular":
                    return sortByMostPopular(searchQuery, isCategory);
                case "Before 1950":
                    return bookList.Where(book => int.Parse(book.date.Split('/')[2]) < 1950).ToList();
                case "1950 - 2000":
                    return bookList.Where(book => int.Parse(book.date.Split('/')[2]) >= 1950 && int.Parse(book.date.Split('/')[2]) <= 2000).ToList();
                case "After 2000":
                    return bookList.Where(book => int.Parse(book.date.Split('/')[2]) > 2000).ToList();
                case "Less than $10":
                    return bookList.Where(book => book.price < 10).ToList();
                case "$10 - $20":
                    return bookList.Where(book => book.price >= 10 && book.price <= 20).ToList();
                case "$20 - $30":
                    return bookList.Where(book => book.price >= 20 && book.price <= 30).ToList();
                case "Above $30":
                    return bookList.Where(book => book.price > 30).ToList();
                case "Below 10":
                    return bookList.Where(book => book.ageLimitation < 10).ToList();
                case "10 - 18":
                    return bookList.Where(book => book.ageLimitation >= 10 && book.ageLimitation < 18).ToList();
                case "Above 18":
                    return bookList.Where(book => book.ageLimitation >= 18).ToList();
                case "Paperback":
                    return bookList.Where(book => book.format == "Paperback").ToList();
                case "Hardcover":
                    return bookList.Where(book => book.format == "Hardcover").ToList();
                default:
                    return bookList;
            }
        }

        public List<Book> sortByMostPopular(string searchQuery, int isCategory)
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
    }
}
