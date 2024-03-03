using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;

namespace BookHeaven.Controllers
{
    public class SearchResultsController : Controller
    {
        public IActionResult showSearchResults(string searchQuery)
        {
            SearchResults searchResults = new SearchResults(searchQuery);
            searchResults = SQLHelper.SQLSearchBook(searchResults);

            return View("SearchResultsView", searchResults);
        }

        public IActionResult showCategoryResults(string searchQuery)
        {
            SearchResults searchResults = new SearchResults(searchQuery);
            searchResults = SQLHelper.SQLSearchCategory(searchResults);

            return View("SearchResultsView", searchResults);
        }
    }
}
