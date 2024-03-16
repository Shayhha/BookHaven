namespace BookHeaven.Models
{
    public class CartItem
    {
        public Book book;
        public int amount;

        public CartItem() { }

        public CartItem(Book _book, int _amount)
        {
            book = _book;
            amount = _amount;
        }


        // Old code:

        // public static List<Book> listOfBooks = new List<Book>();

        //public static bool addBookToCart(int bookId)
        //{
        //    Book book = SQLHelper.SQLSearchBookById(bookId);
        //    if (book != null)
        //    {
        //        listOfBooks.Add(book);
        //        Console.WriteLine("The book '" + book.name + "' has been added to the cart");
        //        return true;
        //    }
        //    return false;
        //}
    }
}
