using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;
using System.Text.RegularExpressions;

namespace BookHeaven.Controllers
{
    public class SearchResultsController : Controller
    {
        public IActionResult showSearchResults(string searchQuery)
        {
            SearchResults searchResults = new SearchResults(searchQuery);

            if (isSearchQueryBookId(searchQuery))
                searchResults = SQLHelper.SQLSearchBook(searchResults, false);
            else
                searchResults = SQLHelper.SQLSearchBook(searchResults);

            return View("SearchResultsView", searchResults);
        }

        public IActionResult showCategoryResults(string searchQuery)
        {
            SearchResults searchResults = new SearchResults(searchQuery);
            searchResults = SQLHelper.SQLSearchCategory(searchResults);

            return View("SearchResultsView", searchResults);
        }

        private bool isSearchQueryBookId(string searchQuery)
        {
            string pattern = "^[0-9]+$";
            return Regex.IsMatch(searchQuery, pattern);
        }
    }
}
