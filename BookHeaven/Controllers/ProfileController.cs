using System.Text.RegularExpressions;
using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult showEditProfileView()
        {

            return View("EditProfileView", Models.User.currentUser);
        }


        private bool userAddressValidation(Address address)
        {
            if (address.country != "" && address.city != "" && address.street != "" && address.apartNum != 0)
            {
                int flag = 0;
                string errorMessage = "";

                if (!Regex.IsMatch(address.country, @"^[a-zA-Z]{2,25}$"))
                {
                    errorMessage += "Invalid Country. ";
                    flag = 1;
                }

                if (!Regex.IsMatch(address.city, @"^[a-zA-Z]{2,25}$")) {
                    errorMessage += "Invalid City. ";
                    flag = 1;
                }

                if (!Regex.IsMatch(address.street, @"^[a-zA-Z]{2,25}$")) {
                    errorMessage += "Invalid Street. ";
                    flag = 1;
                }

                if (address.apartNum == 0)
                {
                    errorMessage += "Invalid Apartment Number. ";
                    flag = 1;
                }


                if (flag == 1)
                {
                    ModelState.AddModelError("address", errorMessage);
                    return false;
                }

                return true;
            }

            if ((address.country == "" || address.country == null) &&
                (address.city == "" || address.city == null) &&
                (address.street == "" || address.street == null) &&
                address.apartNum == 0)
                return true;

            return false;
        }

        private bool userCreditCardValidation(CreditCard creditCard)
        {
            int flag = 0;
            string errorMessage = "";

            if (creditCard.number != 0 && creditCard.date != "" && creditCard.ccv != 0)
            {
                if (!Regex.IsMatch(creditCard.number.ToString(), @"^\d{16}$"))
                {
                    errorMessage += "Invalid Card Number. ";
                    flag = 1;
                }

                if (!Regex.IsMatch(creditCard.date, @"^(0[1-9]|1[0-2])\/\d{2}$"))

                {
                    errorMessage += "Invalid Experation Date. ";
                    flag = 1;
                }

                if (!Regex.IsMatch(creditCard.ccv.ToString(), @"^[1-9]\d{2}$"))
                {
                    errorMessage += "Invalid CCV Number. ";
                    flag = 1;
                }

                if (flag == 1)
                {
                    ModelState.AddModelError("creditCard", errorMessage);
                    return false;
                }
                return true;
            }

            if (creditCard.number == 0 && (creditCard.date == "" || creditCard.date == null) && creditCard.ccv == 0)
                return true;

            return false;
        }

        public IActionResult CheckUsersInput(User user)
        {
            ModelState.Clear();

            // Perform custom user info validation
            if (user.email == null || !Regex.IsMatch(user.email, @"^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$"))
                ModelState.AddModelError("email", "Email can't be null.");

            if (user.fname == null || !Regex.IsMatch(user.fname, @"^[a-zA-Z]{2,20}$"))
                ModelState.AddModelError("fname", "First can't be null.");

            if (user.lname == null || !Regex.IsMatch(user.lname, @"^[a-zA-Z]{2,20}$"))
                ModelState.AddModelError("lname", "Last can't be null.");

            // Perform custom address validation
            if (!userAddressValidation(user.address))
            {
                ModelState.AddModelError("address", "Address validation failed. ");
            }

            // Perform custom credit card validation
            if (!userCreditCardValidation(user.creditCard))
            {
                ModelState.AddModelError("creditCard", "Credit card validation failed. ");
            }

            if (!ModelState.IsValid)
            {
                return View("EditProfileView", user);
            }

            if (SaveUserData(user))
                return View("ProfileView", user);
            else
            {
                ViewBag.generalErrorMessage = "Unable to save the changes, something went wrong please try again.";
                return View("EditProfileView", user);
            }
        }

        public bool SaveUserData(User user)
        {
            return true;
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

