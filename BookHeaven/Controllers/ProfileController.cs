using System.Net;
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

                if (address.country == null || !Regex.IsMatch(address.country, @"^[a-zA-Z]{2,25}$"))
                {
                    errorMessage += "Invalid Country. ";
                    flag = 1;
                }

                if (address.city == null || !Regex.IsMatch(address.city, @"^[a-zA-Z]{2,25}$")) {
                    errorMessage += "Invalid City. ";
                    flag = 1;
                }

                if (address.street == null || !Regex.IsMatch(address.street, @"^[a-zA-Z]{2,25}$")) {
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
                else
                {
                    address.userId = Models.User.currentUser.userId;
                    return true;
                }
            }

            if ((address.country == "" || address.country == null) &&
                (address.city == "" || address.city == null) &&
                (address.street == "" || address.street == null) &&
                address.apartNum == 0)
            {
                if (Models.User.currentUser.address != null)
                {
                    ModelState.AddModelError("address", "You can't delete the address in this page.");
                    return false;
                }

                address.userId = Models.User.currentUser.userId;
                return true;
            }

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
                else
                {
                    creditCard.userId = Models.User.currentUser.userId;
                    return true;
                }
            }

            if (creditCard.number == 0 && (creditCard.date == "" || creditCard.date == null) && creditCard.ccv == 0)
            {
                if (Models.User.currentUser.creditCard != null)
                {
                    ModelState.AddModelError("creditCard", "You can't delete the credit card in this page.");
                    return false;
                }

                creditCard.userId = Models.User.currentUser.userId;
                return true;
            }

            return false;
        }

        public IActionResult CheckUsersInput(User user)
        {
            ModelState.Clear();
            user.userId = Models.User.currentUser.userId;

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
            if ((user.address.country == "" || user.address.country == null) &&
                (user.address.city == "" || user.address.city == null) &&
                (user.address.street == "" || user.address.street == null) &&
                user.address.apartNum == 0)
            {
                user.address = null;
            }

            if (user.creditCard.number == 0 && (user.creditCard.date == "" || user.creditCard.date == null) && user.creditCard.ccv == 0)
            {
                user.creditCard = null;
            }

            return Models.User.currentUser.updateInfo(user);
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

