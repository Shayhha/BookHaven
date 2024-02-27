using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;
using System.Data.SqlClient;

namespace BookHeaven.Controllers
{
    public class AdminHomeController : Controller
    {
        public IConfiguration _configuration;
        public string connectionString = "";
        public AdminHomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("myConnection");
        }

        public IActionResult showAdminHome()
        {
            Login login = new Login(); 
            return View("AdminHomeView", login);
        }

        public IActionResult tryToLogin(Login login)
        {
            if (ModelState.IsValid)
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string query = "SELECT * FROM Users WHERE email = @email AND password = @password;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", login.email);
                        command.Parameters.AddWithValue("@password", login.password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                // User found in the database
                                return View("../UserHome/UserHomeView");
                            }
                            else
                            {
                                // User not found in the database
                                return View("AdminHomeView", login);
                            }
                        }
                    }
                }             
            }
            else
            {
                return View("AdminHomeView", login);
            }
        }
    }
}
