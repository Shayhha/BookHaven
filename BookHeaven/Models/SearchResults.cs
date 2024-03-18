using BookHeaven.Models;

namespace BookHeaven.Models
{
    public class SearchResults
    {
        // maybe we can check here for sql injections?
        public string searchQuery { get; set; }
        public int isCategory;
        public Book book;
        public List<Book> books;

        public SearchResults() { }

        public SearchResults(string _searchQuery, int _isCategory = 0)
        {
            searchQuery = _searchQuery;
            isCategory = _isCategory;
            book = new Book();
            books = new List<Book>();
        }
    }
}
