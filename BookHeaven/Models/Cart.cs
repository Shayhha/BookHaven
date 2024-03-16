namespace BookHeaven.Models
{
    public class Cart
    {
        public static List<Book> listOfBooks = new List<Book>();

        public static bool addBookToCart(int bookId)
        {
            Book book = SQLHelper.SQLSearchBookById(bookId);
            if (book != null)
            {
                listOfBooks.Add(book);
                Console.WriteLine("The book '" + book.name + "' has been added to the cart");
                return true;
            }
            return false;
        }
    }
}
