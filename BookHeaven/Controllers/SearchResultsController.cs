using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;

namespace BookHeaven.Controllers
{
    public class SearchResultsController : Controller
    {
        public IActionResult showSearchResults(string searchQuery)
        {
            SearchResults searchResults = new SearchResults(searchQuery);

            // some sql here to find all the elements that fit the search query?

            return View("SearchResultsView", searchResults);
        }
    }
}
