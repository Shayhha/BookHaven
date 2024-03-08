using BookHeaven.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

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
                        return new CreditCard(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3));
                    }
                }
            }
            return null; //return null if no credit card was found for the given userId
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
                        User user = new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)); //create the user object from db
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
        /// Function for searching books by name or id in the database, returns an initialized SearchResults object with the list of books
        /// </summary>
        /// <param name="searchResults"></param>
        /// <param name="isName"></param>
        /// <returns></returns>
        public static SearchResults SQLSearchBook(SearchResults searchResults, bool isName=true) 
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
                string query = @"INSERT INTO CreditCard(userId, number, date, ccv) VALUES(@userId, @number, @date, @ccv);";

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
                string query = @"UPDATE CreditCard SET number = @number, date = @date, ccv = @ccv WHERE userId = @userId;";

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
    }
}

