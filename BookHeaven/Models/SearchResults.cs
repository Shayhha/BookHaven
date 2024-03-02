using BookHeaven.Models;

namespace BookHeaven.Models
{
    public class SearchResults
    {
        // maybe we can check here for sql injections?
        public string searchQuery { get; set; }

        public Book book;
        public List<Book> books;
        public Cart cart;

        public SearchResults() { }

        public SearchResults(string _searchQuery)
        {
            searchQuery = _searchQuery;
            book = new Book();
            books = new List<Book>();
            cart = new Cart();
        }

        // here there will be more functions of adding items to cart, searching for items maybe? 
    }
}
