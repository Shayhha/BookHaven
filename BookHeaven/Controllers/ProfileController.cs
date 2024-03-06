using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookHeaven.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult showProfileView()
        {
            return View("ProfileView");
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

