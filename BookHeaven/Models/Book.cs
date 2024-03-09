namespace BookHeaven.Models
{
    public class Book
    {
        public string name {  get; set; }
        public string author { get; set; }
        public string date { get; set; }
        public int bookId { get; set; }
        public string category { get; set; }
        public string format { get; set; }
        public float price { get; set; }
        public int stock { get; set; }
        public string imageUrl { get; set; }
        public int ageLimitation { get; set; }
        public float salePrice {  get; set; }

        public Book() { }

        ///this is a constructor for Books that we use to initialize books from db and view them in the website
        public Book(string name, string author, string date, int bookId, string category, string format, float price, int stock, string imageUrl, int ageLimitation, float salePrice = 0)
        {
            this.name = name;
            this.author = author;
            this.date = date;
            this.bookId = bookId;
            this.category = category;
            this.format = format;
            this.price = price;
            this.stock = stock;
            this.imageUrl = imageUrl;
            this.ageLimitation = ageLimitation;
            this.salePrice = salePrice;
        }

        ///this is constructor for adding new books to the db by the admin 
        public Book(string name, string author, string date, string category, string format, float price, int stock, string imageUrl, int ageLimitation)
        {
            this.name = name;
            this.author = author;
            this.date = date;
            this.bookId = 0;
            this.category = category;
            this.format = format;
            this.price = price;
            this.stock = stock;
            this.imageUrl = imageUrl;
            this.ageLimitation = ageLimitation;
            this.salePrice = 0;
        }

        //These functions are for admin for making changes in the books//

        /// <summary>
        /// Function for adding book, only for admin
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public static bool addBook(Book book)
        {
            if (Models.User.currentUser.isAdmin == true)
            {
                if (book != null)
                {
                    if (SQLHelper.SQLAddBook(book) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Function for deleting book, only for admin
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public static bool deleteBook(int bookId)
        {
            if (Models.User.currentUser.isAdmin == true)
            {
                if (SQLHelper.SQLDeleteBook(bookId) == true)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Function for updating book's price or salePrice
        /// If isSale is true we change the salePrice, else we change original price
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="newPrice"></param>
        /// <param name="isSale"></param>
        /// <returns></returns>
        public static bool updateBookPrice(int bookId, float newPrice, bool isSale = false)
        {
            if (Models.User.currentUser.isAdmin == true)
            {
                if (SQLHelper.SQLUpdateBookPrice(bookId, newPrice, isSale) == true)
                {
                    return true;
                } 
            }
            return false;
        }

        /// <summary>
        /// Function for updating book's stock, for buying an item or restock an item
        /// if admin wants to restock we give it true
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="stock"></param>
        /// <param name="isRestock"></param>
        /// <returns></returns>
        public static bool updateBookStock(int bookId, int stock, bool isRestock = false)
        {
            if (SQLHelper.SQLUpdateBookStock(bookId, stock, isRestock) == true)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function for updating book's category
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool updateBookCategory(int bookId, string category)
        {
            if (Models.User.currentUser.isAdmin == true)
            {
                if (SQLHelper.SQLUpdateBookCategory(bookId, category) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
