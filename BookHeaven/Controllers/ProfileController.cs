using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookHeaven.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult showProfileView()
        {
            return View("ProfileView");
        }

        public IActionResult showSettingsView()
        {
            return View("SettingsView");
        }



        [HttpPost]
        public async Task<IActionResult> SaveChanges()
        {
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                string jsonString = await reader.ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(jsonString);

                if (data != null)
                {
                    // Here we check what data needs to be updated and call the appropriate sql methods

                    // You can access the data like so:
                    string email = data.email;
                    Console.WriteLine("email = " + email);


                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }

        }

        [HttpPost]
        public IActionResult DeleteCreditCard()
        {
            // Logic to delete credit card information from the database
            // Example:
            //var user = GetUserFromSession(); // Implement this according to your application's logic
            //user.DeleteCreditCardInfo();
            //SaveChanges(); // Assuming Entity Framework Core

            return Ok(); // Return HTTP 200 OK status if successful
        }

        [HttpPost]
        public IActionResult DeleteAddress()
        {
            // Logic to delete address information from the database
            // Example:
            // var user = GetUserFromSession(); // Implement this according to your application's logic
            // user.DeleteAddressInfo();
            // SaveChanges(); // Assuming Entity Framework Core

            return Ok(); // Return HTTP 200 OK status if successful
        }
    }
}

