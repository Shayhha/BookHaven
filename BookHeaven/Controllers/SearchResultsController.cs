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
            searchResults = SQLHelper.SQLSearchBook(searchResults);
            if (searchResults.books.Count() > 0)
            {
                foreach (Book book in searchResults.books)
                {
                    Console.WriteLine(book.name);
                }
            } else
            {
                Console.WriteLine("empty list");
            }
                

            return View("SearchResultsView", searchResults);
        }
    }
}
