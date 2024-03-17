using BookHeaven.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BookHeaven.Models
{
    public static class SQLHelper
    {
        //initial configuration of the SQL connection
        public static IConfiguration _configuration;
        public static string connectionString = "";
        //end of SQL connection configuration

        /// <summary>
        /// Function for configuring the initial connection with the database, we call in program.cs
        /// </summary>
        /// <param name="configuration"></param>
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("myConnection");
        }


        /// <summary>
        /// Function to generate SHA-256 hash for strings
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hash = sha256Hash.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Function for searching user in db and return true if he exists, else false
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool SQLCheckEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE email = @email;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            return true; //if we found a matching email we return true
                        else
                            return false; // else we didn't find so we return false
                    }
                }
            }
        }

        /// <summary>
        /// Function for searching address of user in db and return true if it exists, else false
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static bool SQLCheckAddress(int userId, SqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM Address WHERE userId = @userId;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                int count = (int)command.ExecuteScalar();
                return count > 0; //return true if the count is greater than 0, indicating the user has an address
            }
        }

        /// <summary>
        /// Function for initializing address object from db
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static Address? SQLInitAddress(int userId, SqlConnection connection)
        {
            string query = "SELECT * FROM Address WHERE userId = @userId;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        //initialize address object with database information
                        return new Address(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4));
                    }
                }
            }
            return null; //return null if no address was found for the given userId
        }

        /// <summary>
        /// Function for checking if user has credit card in db
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static bool SQLCheckCreditCard(int userId, SqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM CreditCards WHERE userId = @userId;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                int count = (int)command.ExecuteScalar();
                return count > 0; //return true if the count is greater than 0, indicating the user has a credit card
            }
        }

        /// <summary>
        /// Function for creating credit card object from db
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static CreditCard? SQLInitCreditCard(int userId, SqlConnection connection)
        {
            string query = "SELECT * FROM CreditCards WHERE userId = @userId;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        //initialize credit card object with database information
                        return new CreditCard(reader.GetInt32(0), reader.GetInt64(1), reader.GetString(2), reader.GetInt32(3));
                    }
                }
            }
            return null; //return null if no credit card was found for the given userId
        }

        /// <summary>
        /// Function for checking if user has cart items in database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static bool SQLCheckCart(int userId, SqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM Cart WHERE userId = @userId;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                int count = (int)command.ExecuteScalar();
                return count > 0; //return true if the count is greater than 0, indicating the user has items in cart
            }
        }

        /// <summary>
        /// Function for initializing cartItems for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static List<CartItem> SQLInitCartItems(int userId, SqlConnection connection)
        {
            string query = "SELECT * FROM Cart WHERE userId = @userId;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<CartItem> cartItems = new List<CartItem>(); //our actual cart list
                    List<int> bookIdList = new List<int>(); //temp list to hold bookId's to initialize later
                    while (reader.Read())
                    {
                        CartItem cartItem = new CartItem(reader.GetInt32(2)); //create cartItem with book set to null and with amount
                        cartItems.Add(cartItem); //add cartItem to list
                        bookIdList.Add(reader.GetInt32(1)); //add all book id's to our temp list for initializing book objects
                    }
                    reader.Close(); //close the reader 
                    for (int i = 0; i < cartItems.Count; i++) //iterate over cartItems list and initialize all book objects
                    {
                        int bookId = bookIdList[i]; //get bookId from list
                        Book book = SQLSearchBookById(bookId, connection); //get book object
                        cartItems[i].book = book; // set book object in our carteItem
                    }
                    return cartItems; //return initialized cartItems list
                }
            }
        }

        /// <summary>
        /// Function for creating user object for our use later in the website
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static User? SQLCreateUserObj(int userId, SqlConnection connection)
        {
            string query = "SELECT * FROM UserInfo WHERE userId = @userId;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) //if true we found user in UserInfo table
                    {
                        User user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetBoolean(4)); //create the user object from db
                        reader.Close(); //we close our reader before next process to avoid open reader exceptions
                        //now we check if user has address and credit card configured in his account
                        if (SQLCheckAddress(userId, connection))
                        {
                            user.address = SQLInitAddress(userId, connection); //if the user has an address, initialize the address object
                        }
                        if (SQLCheckCreditCard(userId, connection))
                        {
                            user.creditCard = SQLInitCreditCard(userId, connection); //if the user has credit card, initialize the creditCard object
                        }
                        if (SQLCheckCart(userId, connection))
                        {
                            user.cartItems = SQLInitCartItems(userId, connection); //initialize the cart of user
                        }
                        return user; //we return the initialized user object
                    }
                }
            }
            return null; //return null if no user was found for the given userId
        }

        /// <summary>
        /// Function for login, searches the database for matching user and returns its unique object, else returns null
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static User? SQLLogin(Login login)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE email = @email AND password = @password;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", login.email);
                    command.Parameters.AddWithValue("@password", ToSHA256(login.password));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read(); // Read the first row
                            int userId = reader.GetInt32(0); //get userId from Users table
                            reader.Close(); //we close our reader before next process to avoid open reader exceptions
                            User user = SQLCreateUserObj(userId, connection); //send userId and connection to our function to create user obj 
                            return user != null ? user : null; //return user object if user is not null, else we return null indicating of an error finding user in db
                        }
                        else
                            return null; // else user was not found in the database
                    }
                }
            }
        }

        /// <summary>
        /// Function for signup, adds a new user to Users and UserInfo tables and returns its unique id, else returns empty string
        /// </summary>
        /// <param name="signup"></param>
        /// <returns></returns>
        public static User? SQLSignup(Signup signup)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction(); //begin a transaction

                try
                {
                    string query = @"
                        INSERT INTO Users(email, password) OUTPUT INSERTED.UserId VALUES(@email, @password);
                        INSERT INTO UserInfo(email, fname, lname) VALUES(@email, @fname, @lname);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Transaction = transaction; //associate the command with the transaction

                        command.Parameters.AddWithValue("@email", signup.email);
                        command.Parameters.AddWithValue("@password", ToSHA256(signup.password));
                        command.Parameters.AddWithValue("@fname", signup.firstName);
                        command.Parameters.AddWithValue("@lname", signup.lastName);

                        //execute the command and retrieve the UserId directly
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            transaction.Commit(); //commit the transaction if both insertions are successful
                            return new User(int.Parse(result.ToString()), signup.email, signup.firstName, signup.lastName);
                        }
                        else
                        {
                            transaction.Rollback(); //rollback the transaction if the second insertion fails
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); //rollback the transaction in case of an exception
                    throw ex; //rethrow the exception
                }
            }
        }

        /// <summary>
        /// Function for updating user info in UserInfo db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool SQLUpdateUserInfo(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    UPDATE UserInfo SET email = @email, fname = @fname, lname = @lname WHERE userId = @userId;
                    UPDATE Users SET email = @email WHERE userId = @userId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", user.userId);
                    command.Parameters.AddWithValue("@email", user.email);
                    command.Parameters.AddWithValue("@fname", user.fname);
                    command.Parameters.AddWithValue("@lname", user.lname);

                    //execute the command and check if we updated the userInfo
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //userInfo successfully updated
                    else
                        return false; //failed to update userInfo
                }
            }
        }

        /// <summary>
        /// Function for searching books by name or id in the database, returns an initialized SearchResults object with the list of books
        /// </summary>
        /// <param name="searchResults"></param>
        /// <param name="isName"></param>
        /// <returns></returns>
        public static SearchResults SQLSearchBook(SearchResults searchResults, bool isName = true)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query;
                if (isName) //if true we search by book name
                    query = "SELECT * FROM Books WHERE name LIKE '%' + @searchQuery + '%';";
                else //else we search by book id
                    query = "SELECT * FROM Books WHERE bookId = @searchQuery;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchQuery", searchResults.searchQuery);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4),
                                reader.GetString(5), reader.GetFloat(6), reader.GetInt32(7), reader.GetString(8), reader.GetInt32(9), reader.GetFloat(10));
                            searchResults.books.Add(book);
                        }
                    }
                }
            }
            return searchResults;
        }


        /// <summary>
        /// Function for searching popular books by category in the database, returns an initialized SearchResults object with the list of books
        /// </summary>
        /// <param name="searchResults"></param>
        /// <returns></returns>
        public static List<Book> SQLSearchPopularBook(string searchQuery)
        {
            List<Book> popularBooks; //declare list of popular books to return
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT b.name, b.author, b.date, b.bookId, b.category, b.format, b.price, b.stock, b.imageUrl, b.ageLimitation, b.salePrice, 
                                        ISNULL(SUM(od.quantity), 0) AS totalOrdered
                                FROM Books b
                                LEFT JOIN OrderDetails od ON b.bookId = od.bookId
                                WHERE b.name LIKE '%' + @searchQuery + '%'
                                GROUP BY b.name, b.author, b.date, b.bookId, b.category, b.format, b.price, b.stock, b.imageUrl, b.ageLimitation, b.salePrice
                                ORDER BY totalOrdered DESC;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchQuery", searchQuery);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        popularBooks = new List<Book>(); //initialize the list of popular books
                        while (reader.Read()) //add each book we found
                        {
                            Book book = new Book(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4),
                                reader.GetString(5), reader.GetFloat(6), reader.GetInt32(7), reader.GetString(8), reader.GetInt32(9), reader.GetFloat(10));
                            popularBooks.Add(book);
                        }
                    }
                }
            }
            return popularBooks;
        }


        /// <summary>
        /// Function for searching for a SINGLE BOOK by it's bookId in the database, returns an initialized Book object with all of the information about the book from the database
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns>Might return null if the book was not found. Otherwise returns an initialized Book object</returns>
        public static Book? SQLSearchBookById(int bookId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Books WHERE bookId = @searchQuery;";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchQuery", bookId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Book(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4),
                                reader.GetString(5), reader.GetFloat(6), reader.GetInt32(7), reader.GetString(8), reader.GetInt32(9), reader.GetFloat(10));
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Function for searching book my id with sql connection string
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static Book? SQLSearchBookById(int bookId, SqlConnection connection)
        {
            string query = "SELECT * FROM Books WHERE bookId = @searchQuery;";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchQuery", bookId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Book(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4),
                            reader.GetString(5), reader.GetFloat(6), reader.GetInt32(7), reader.GetString(8), reader.GetInt32(9), reader.GetFloat(10));
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Function for searching books by category in the database, returns an initialized SearchResults object with the list of books
        /// </summary>
        /// <param name="searchResults"></param>
        /// <returns></returns>
        public static SearchResults SQLSearchCategory(SearchResults searchResults)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Books WHERE category LIKE '%' + @searchQuery + '%';";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchQuery", searchResults.searchQuery);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4),
                                reader.GetString(5), reader.GetFloat(6), reader.GetInt32(7), reader.GetString(8), reader.GetInt32(9), reader.GetFloat(10));
                            searchResults.books.Add(book);
                        }
                    }
                }
            }
            return searchResults;
        }


        /// <summary>
        /// Function for searching popular books by category
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public static List<Book> SQLSearchPopularCategory(string searchQuery)
        {
            List<Book> popularBooks; //declare list of popular books to return
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT b.name, b.author, b.date, b.bookId, b.category, b.format, b.price, b.stock, b.imageUrl, b.ageLimitation, b.salePrice, 
                                        ISNULL(SUM(od.quantity), 0) AS totalOrdered
                                FROM Books b
                                LEFT JOIN OrderDetails od ON b.bookId = od.bookId
                                WHERE b.category LIKE '%' + @searchQuery + '%'
                                GROUP BY b.name, b.author, b.date, b.bookId, b.category, b.format, b.price, b.stock, b.imageUrl, b.ageLimitation, b.salePrice
                                ORDER BY totalOrdered DESC;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchQuery", searchQuery);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        popularBooks = new List<Book>(); //initialize the list of popular books
                        while (reader.Read()) //add each book we found
                        {
                            Book book = new Book(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4),
                                reader.GetString(5), reader.GetFloat(6), reader.GetInt32(7), reader.GetString(8), reader.GetInt32(9), reader.GetFloat(10));
                            popularBooks.Add(book);
                        }
                    }
                }
            }
            return popularBooks;
        }


        /// <summary>
        /// Function to check if book already exists
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool SQLCheckBook(string bookName, string bookAuthor, string bookDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Books WHERE name = @name AND author = @author AND date = @date;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", bookName);
                    command.Parameters.AddWithValue("@author", bookAuthor);
                    command.Parameters.AddWithValue("@date", bookDate);
                    int count = (int)command.ExecuteScalar();
                    return count > 0; //return true if the count is greater than 0, indicating the book already exists
                }
            }
        }


        /// <summary>
        /// Returns an address for the userId, if didn't find any it will return "null"
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Address? SQLSearchAddress(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Address WHERE userId = @userId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return new Address(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4));
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>
        /// Function for adding address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool SQLAddAddress(Address address)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Address(userId, country, city, street, apartNum) VALUES(@userId, @country, @city, @street, @apartNum);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", address.userId);
                    command.Parameters.AddWithValue("@country", address.country);
                    command.Parameters.AddWithValue("@city", address.city);
                    command.Parameters.AddWithValue("@street", address.street);
                    command.Parameters.AddWithValue("@apartNum", address.apartNum);

                    //execute the command and check if we added the address 
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //address successfully added
                    else
                        return false; //failed to add the address
                }
            }
        }

        /// <summary>
        /// Function for updating current user's address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool SQLUpdateAddress(Address address)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE Address SET country = @country, city = @city, street = @street, apartNum = @apartNum WHERE userId = @userId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", address.userId);
                    command.Parameters.AddWithValue("@country", address.country);
                    command.Parameters.AddWithValue("@city", address.city);
                    command.Parameters.AddWithValue("@street", address.street);
                    command.Parameters.AddWithValue("@apartNum", address.apartNum);

                    //execute the command and check if we updated the address 
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //address successfully updated
                    else
                        return false; //failed to update the address
                }
            }
        }

        /// <summary>
        /// Function for deleting current user's address
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool SQLDeleteAddress(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"DELETE FROM Address WHERE userId = @userId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    //execute the command and check if we updated the address 
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //address successfully deleted
                    else
                        return false; //failed to delete the address
                }
            }
        }

        /// <summary>
        /// Function for searching user's saved credit card
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static CreditCard? SQLSearchCreditCard(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM CreditCard WHERE userId = @userId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return new CreditCard(reader.GetInt32(0), reader.GetInt64(1), reader.GetString(2), reader.GetInt32(3));
                        else
                            return null;
                    }
                }
            }
        }

        /// <summary>
        /// Function for adding credit card for a user
        /// </summary>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        public static bool SQLAddCreditCard(CreditCard creditCard)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO CreditCards(userId, number, date, ccv) VALUES(@userId, @number, @date, @ccv);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", creditCard.userId);
                    command.Parameters.AddWithValue("@number", creditCard.number);
                    command.Parameters.AddWithValue("@date", creditCard.date);
                    command.Parameters.AddWithValue("@ccv", creditCard.ccv);

                    //execute the command and check if we added the credit card
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //credit card successfully added
                    else
                        return false; //failed to add the credit card
                }
            }
        }

        /// <summary>
        /// Function for updating user's credit card info 
        /// </summary>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        public static bool SQLUpdateCreditCard(CreditCard creditCard)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE CreditCards SET number = @number, date = @date, ccv = @ccv WHERE userId = @userId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", creditCard.userId);
                    command.Parameters.AddWithValue("@number", creditCard.number);
                    command.Parameters.AddWithValue("@date", creditCard.date);
                    command.Parameters.AddWithValue("@ccv", creditCard.ccv);

                    //execute the command and check if we updated the credit card
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //credit card successfully updated
                    else
                        return false; //failed to update the credit card
                }
            }
        }

        /// <summary>
        /// Function for deleting user's credit card
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool SQLDeleteCreditCard(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"DELETE FROM CreditCard WHERE userId = @userId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    //execute the command and check if we deleted the credit card
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //credit card successfully deleted
                    else
                        return false; //failed to delete the credit card
                }
            }
        }

        /// <summary>
        /// Function for adding item to cart in database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        public static bool SQLAddCartItem(int userId, CartItem cartItem)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Cart(userId, bookId, amount) VALUES(@userId, @bookId, @amount);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@bookId", cartItem.book.bookId);
                    command.Parameters.AddWithValue("@amount", cartItem.amount);

                    //execute the command and check if we added the credit card
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //credit card successfully added
                    else
                        return false; //failed to add the credit card
                }
            }
        }

        /// <summary>
        /// Function for updating amount in cart for an item for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool SQLUpdateCartItem(int userId, CartItem cartItem)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE Cart SET amount = @amount WHERE userId = @userId AND bookId = @bookId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@bookId", cartItem.book.bookId);
                    command.Parameters.AddWithValue("@amount", cartItem.amount);

                    //execute the command and check if we updated the credit card
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //credit card successfully updated
                    else
                        return false; //failed to update the credit card
                }
            }
        }

        /// <summary>
        /// Function for deleting cart item for a user from db
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public static bool SQLDeleteCartItem(int userId, int bookId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"DELETE FROM Cart WHERE userId = @userId AND bookId = @bookId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@bookId", bookId);

                    //execute the command and check if we deleted the credit card
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //credit card successfully deleted
                    else
                        return false; //failed to delete the credit card
                }
            }
        }

        /// <summary>
        /// Function for changing password for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public static bool SQLUpdatePassword(int userId, string oldPassword, string newPassword)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE Users SET password = @newPassword WHERE userId = @userId AND password = @oldPassword";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@oldPassword", ToSHA256(oldPassword));
                    command.Parameters.AddWithValue("@newPassword", ToSHA256(newPassword));

                    //execute the command and check if we updated the password
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //password successfully changed
                    else
                        return false; //failed to change password, maybe because the old password doesn't match
                }
            }
        }

        /// <summary>
        /// Function for adding book into the Books db
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public static bool SQLAddBook(Book book)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO Books(name, author, date, category, format, price, stock, imageUrl, ageLimitation, salePrice)
                                 VALUES(@name, @author, @date, @category, @format, @price, @stock, @imageUrl, @ageLimitation, @salePrice);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", book.name);
                    command.Parameters.AddWithValue("@author", book.author);
                    command.Parameters.AddWithValue("@date", book.date);
                    command.Parameters.AddWithValue("@category", book.category);
                    command.Parameters.AddWithValue("@format", book.format);
                    command.Parameters.AddWithValue("@price", book.price);
                    command.Parameters.AddWithValue("@stock", book.stock);
                    command.Parameters.AddWithValue("@imageUrl", book.imageUrl);
                    command.Parameters.AddWithValue("@ageLimitation", book.ageLimitation);
                    command.Parameters.AddWithValue("@salePrice", book.salePrice);

                    //execute the command and check if we added the book
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //book successfully added
                    else
                        return false; //failed to add the book
                }
            }
        }

        /// <summary>
        /// Function for deleting a book from the Books db
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public static bool SQLDeleteBook(int bookId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"DELETE FROM Books WHERE bookId = @bookId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookId", bookId);

                    //execute the command and check if we deleted the book
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //book successfully deleted
                    else
                        return false; //failed to delete the book
                }
            }
        }

        /// <summary>
        /// Function for updating book information by Book object 
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public static bool SQLUpdateBook(Book book)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE Books SET name = @name, author = @author, date = @date, category = @category, format = @format, price = @price,
                                stock = @stock, imageUrl = @imageUrl, ageLimitation = @ageLimitation, salePrice = @salePrice WHERE bookId = @bookId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", book.name);
                    command.Parameters.AddWithValue("@author", book.author);
                    command.Parameters.AddWithValue("@date", book.date);
                    command.Parameters.AddWithValue("@category", book.category);
                    command.Parameters.AddWithValue("@format", book.format);
                    command.Parameters.AddWithValue("@price", book.price);
                    command.Parameters.AddWithValue("@stock", book.stock);
                    command.Parameters.AddWithValue("@imageUrl", book.imageUrl);
                    command.Parameters.AddWithValue("@ageLimitation", book.ageLimitation);
                    command.Parameters.AddWithValue("@salePrice", book.salePrice);
                    command.Parameters.AddWithValue("@bookId", book.bookId);

                    //execute the command and check if we updated the book category
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //category successfully changed
                    else
                        return false; //failed to change category
                }
            }
        }

        /// <summary>
        /// Function for updating book price or salePrice in Books db
        /// To update salePrice, set isSale to true
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="newPrice"></param>
        /// <param name="isSale"></param>
        /// <returns></returns>
        public static bool SQLUpdateBookPrice(int bookId, float newPrice, bool isSale = false)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query;
                if (!isSale)
                    query = @"UPDATE Books SET price = @price WHERE bookId = @bookId";
                else
                    query = @"UPDATE Books SET salePrice = @price WHERE bookId = @bookId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@price", newPrice);
                    command.Parameters.AddWithValue("@bookId", bookId);

                    //execute the command and check if we updated the book price
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //price successfully changed
                    else
                        return false; //failed to change price
                }
            }
        }

        /// <summary>
        /// Function for adding more stock to a certain book in Books db
        /// If we want to restock this specific book, we give it true for admin to add more stock
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="stock"></param>
        /// <param name="isRestock"></param>
        /// <returns></returns>
        public static bool SQLUpdateBookStock(int bookId, int stock, bool isRestock = false)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query;
                if (isRestock)
                    query = @"UPDATE Books SET stock += @stock WHERE bookId = @bookId";
                else
                    query = @"UPDATE Books SET stock -= @stock WHERE bookId = @bookId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@stock", stock);
                    command.Parameters.AddWithValue("@bookId", bookId);

                    //execute the command and check if we updated the book stock
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return true; //stock successfully changed
                    else
                        return false; //failed to change stock
                }
            }
        }



    }
}



