using BookHeaven.Models;
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
            return View("ProfileView", Models.User.currentUser);
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
                    //set all parameters from dict
                    string email = data.email;
                    string fname = data.fname;
                    string lname = data.lname;
                    //address
                    string country = data.address.country;
                    string city = data.address.city;
                    string street = data.address.street;
                    string apartNum = data.address.apartNum;
                    //credit card
                    string number = data.creditCard.number;
                    string date = data.creditCard.date;
                    string ccv = data.creditCard.ccv;

                    // Here we check what data needs to be updated and call the appropriate sql methods
                    //if (Models.User.currentUser.email != email)
                    //{
                    //    if (SQLHelper.SQLCheckEmail(email)) //means the email is being used by another user
                    //        return Json(new { success = false, failure = "Email is assigned to another user, please provide valid email." }); //return false indicating of an error assigning this email to the current user
                    //}

                    //User tempUser = new User(Models.User.currentUser.userId, email, fname, lname);
                    //if (country != "" && city != "" && street != "")
                    //{
                    //    tempUser.address = new Address(Models.User.currentUser.userId, country, city, street, int.Parse(apartNum));

                    //}
                    //if (number != "")
                    //{
                    //    tempUser.creditCard = new CreditCard(Models.User.currentUser.userId, long.Parse(data.creditCard.number), data.creditCard.date, int.Parse(data.creditCard.ccv));

                    //}
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

