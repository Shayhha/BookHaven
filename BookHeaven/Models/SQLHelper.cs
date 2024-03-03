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
        /// <param name="login"></param>
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
        /// Function for login, searches the database for matching user and returns its unique id, else returns empty string
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static string SQLLogin(Login login)
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
                            string userId = reader["UserId"].ToString(); //get userId from Users table
                            return string.IsNullOrEmpty(userId) ? "" : userId; ;// return user id if we found him
                        }
                        else
                            return ""; // else user was not found in the database
                    }
                }
            }
        }

        /// <summary>
        /// Function for signup, adds a new user to Users and UserInfo tables and returns its unique id, else returns empty string
        /// </summary>
        /// <param name="signup"></param>
        /// <returns></returns>
        public static string SQLSignup(Signup signup)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO UserInfo(email, fname, lname) VALUES(@email, @fname, @lname);
                    INSERT INTO Users(email, password) OUTPUT INSERTED.UserId VALUES(@email, @password);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", signup.email);
                    command.Parameters.AddWithValue("@password", ToSHA256(signup.password));
                    command.Parameters.AddWithValue("@fname", signup.firstName);
                    command.Parameters.AddWithValue("@lname", signup.lastName);

                    // Execute the command and retrieve the UserId directly
                    object result = command.ExecuteScalar();
                    if (result != null)
                        return result.ToString(); // Return the UserId if the user is successfully inserted
                    else
                        return ""; // Return empty string if insertion failed
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
                    query = "SELECT * FROM Books WHERE name LIKE @searchQuery;";
                else //else we search by book id
                    query = "SELECT * FROM Books WHERE bookId = @searchQuery;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchQuery", "%" + searchResults.searchQuery + "%");

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
                string query = "SELECT * FROM Books WHERE category = @searchQuery;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchQuery", "%" + searchResults.searchQuery + "%");

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

    }
}

