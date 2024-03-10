using System.ComponentModel.DataAnnotations;

namespace BookHeaven.Models
{
    public class Book
    {

        public int bookId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Name must only contain letters")]
        [MaxLength(255, ErrorMessage = "Name must be between 1 and 255 characters")]
        public string name { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "Author must only contain letters and spaces")]
        [MaxLength(255, ErrorMessage = "Author must be between 1 and 255 characters")]
        public string author { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [RegularExpression(@"^\d{2}/\d{2}/\d{4}$", ErrorMessage = "Invalid date format. Please use dd/mm/yyyy")]
        public string date { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [RegularExpression("^([A-Za-z]+(,\\s*|$))+$", ErrorMessage = "Category must only contain letters separated by commas")]
        public string category { get; set; }

        [Required(ErrorMessage = "Format is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Format must only contain letters")]
        [MaxLength(255, ErrorMessage = "Format must be between 1 and 255 characters")]
        public string format { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be above 0")]
        public float price { get; set; }

        [Required(ErrorMessage = "Sale Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale Price must be above 0")]
        public float salePrice { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, 1000, ErrorMessage = "Stock must be between 0 and 1000")]
        public int stock { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [MaxLength(500, ErrorMessage = "Image URL must be between 1 and 500 characters")]
        public string imageUrl { get; set; }

        [Required(ErrorMessage = "Age limitation is required")]
        [Range(0, 120, ErrorMessage = "Age limitation must be between 0 and 120")]
        public int ageLimitation { get; set; }



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
