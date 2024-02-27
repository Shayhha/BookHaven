using BookHeaven.Models;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace BookHaven.Models
{
    public static class SQLHelper
    {
        //initial configuration of the SQL connection
        public static IConfiguration _configuration;
        public static string connectionString = "";
        public static void Initialize(IConfiguration configuration) //!!must call this function in the start of the website!!!
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("myConnection");
        }
        //end of SQL connection configuration

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

        public static bool SQLSignup(Signup signup)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO UserInfo VALUES(@email, @fname, @lname);
                    INSERT INTO Users VALUES(@email, @password);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", signup.email);
                    command.Parameters.AddWithValue("@password", ToSHA256(signup.password));
                    command.Parameters.AddWithValue("@fname", signup.fname);
                    command.Parameters.AddWithValue("@lname", signup.lname);

                    int numOfRowsEffected = command.ExecuteNonQuery();
                    if (numOfRowsEffected > 0)
                        return true; // successfully inserted user into tables
                    else
                        return false; // else we failed inserting
                }
            }
        }
    }
}
