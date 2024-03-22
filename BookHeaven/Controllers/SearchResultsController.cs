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


        public IActionResult showFilteredBooks(string filterBy, string searchQuery = "", int isCategory = 0)
        {
            SearchResults searchResults;

            if (searchQuery == "")
                searchResults = new SearchResults("All Books");
            else
                searchResults = new SearchResults(searchQuery, isCategory);

            // Get the books that we have saved in the session variable
            List<Book> bookList = _contx.HttpContext.Session.GetObjectFromJson<List<Book>>("listOfBooks");

            // Call a function that will filter the sessionBooks
            bookList = multiFilterBooks(bookList, filterBy, searchResults.searchQuery, isCategory);

            // Then we add the books in the correct order to the searchResults object and send it to the users screen
            foreach (Book book in bookList)
            {
                searchResults.books.Add(book);
            }

            ViewBag.SelectedFilter = filterBy;
            return View("SearchResultsView", searchResults);
        }

        /// <summary>
        /// Function for sorting given bookList, filters by each filter option in filterBy: "A - Z,Paperback,10$ - 20$" etc.
        /// </summary>
        /// <param name="bookList"></param>
        /// <param name="filterBy"></param>
        /// <param name="searchQuery"></param>
        /// <param name="isCategory"></param>
        /// <returns></returns>
        public List<Book> multiFilterBooks(List<Book> bookList, string filterBy, string searchQuery = "", int isCategory = 0) 
        {
            List<Book> tempBookList = bookList; //save bookList in temp list for sorting
            List<string> filterList = filterBy.Split(',').ToList(); //create a new list from filterBy string
            ViewBag.FilterList = filterList;
            foreach (string filter in filterList) //iterate over list and filter our temp book list with appropriate filters
            {
                tempBookList = filterBooks(tempBookList, filter, searchQuery, isCategory); //call helper function for sorting by each filter
            }
            return tempBookList; //return filtered list
        }

        /// <summary>
        /// Function that receives bookList and filters it by the given filter
        /// </summary>
        /// <param name="bookList"></param>
        /// <param name="filter"></param>
        /// <param name="searchQuery"></param>
        /// <param name="isCategory"></param>
        /// <returns></returns>
        public List<Book> filterBooks(List<Book> bookList, string filter, string searchQuery = "", int isCategory = 0) 
        {
            switch (filter) 
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
                case "Less Than $10":
                    return bookList.Where(book => book.salePrice > 0 ? (book.salePrice < 10) : (book.price < 10)).ToList();
                case "$10 - $20":
                    return bookList.Where(book => book.salePrice > 0 ? (book.salePrice >= 10 && book.salePrice <= 20) : (book.price >= 10 && book.price <= 20)).ToList();
                case "$20 - $30":
                    return bookList.Where(book => book.salePrice > 0 ? (book.salePrice >= 20 && book.salePrice <= 30) : (book.price >= 20 && book.price <= 30)).ToList();
                case "More Than $30":
                    return bookList.Where(book => book.salePrice > 0 ? (book.salePrice > 30) : (book.price > 30)).ToList();
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

        /// <summary>
        /// Function that return list of popular books sorted from most popular to least popular
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="isCategory"></param>
        /// <returns></returns>
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
