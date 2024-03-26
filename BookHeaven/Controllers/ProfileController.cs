using System.Text.RegularExpressions;
using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult showProfileView()
        {
            string message = TempData["GeneralMessage"] as string;
            ViewBag.GeneralMessage = message;
            Models.User.currentUser.orders = SQLHelper.SQLInitUserOrders(Models.User.currentUser.userId);
            ViewBag.cardNumber = saveCardNumberInViewBag();
            return View("ProfileView", Models.User.currentUser);
        }

        public IActionResult handleCancelButton()
        {
            return View("ProfileView", Models.User.currentUser);
        }

        public IActionResult showSettingsView()
        {
            return View("SettingsView");
        }

        public IActionResult showEditProfileView()
        {
            string message = TempData["GeneralMessage"] as string;
            ViewBag.GeneralMessage = message;
            ViewBag.cardNumber = saveCardNumberInViewBag();
            return View("EditProfileView", Models.User.currentUser);
        }

        public IActionResult showChangePasswordView()
        {
            string message = TempData["GeneralMessage"] as string;
            ViewBag.GeneralMessage = message;
            ViewBag.CurrentPass = TempData["CurrentPass"] as string;
            ViewBag.NewPass = TempData["NewPass"] as string;
            ViewBag.ConfirmPass = TempData["ConfirmPass"] as string;
            return View("ChangePasswordView");
        }


        public IActionResult changePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            TempData["CurrentPass"] = currentPassword;
            TempData["newPass"] = newPassword;
            TempData["confirmPass"] = confirmPassword;

            if (newPassword != confirmPassword)
            {
                TempData["GeneralMessage"] = "New password and confirm password are not the same.";
                return RedirectToAction("showChangePasswordView", "Profile");
            }
            else if (currentPassword == newPassword)
            {
                TempData["GeneralMessage"] = "New password must be different from current password.";
                return RedirectToAction("showChangePasswordView", "Profile");
            }
            else
            {
                string pattern = "^(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d!@#$%^&*()-_=+{}\\[\\]|;:'\",.<>?]{6,12}$";
                if (!Regex.IsMatch(newPassword, pattern))
                {
                    TempData["GeneralMessage"] = "New password does not match the password pattern. A password must have at-least one capital letter, at-least one number and be between 6 and 12 chars long.";
                    return RedirectToAction("showChangePasswordView", "Profile");
                }
            }

            if (!SQLHelper.SQLCheckCurrentPassword(Models.User.currentUser.userId, currentPassword))
            {
                TempData["GeneralMessage"] = "Current password in incorrect.";
                return RedirectToAction("showChangePasswordView", "Profile");
            }
            else
            {
                if (!SQLHelper.SQLUpdatePassword(Models.User.currentUser.userId, currentPassword, newPassword))
                {
                    TempData["GeneralMessage"] = "An error occured while saving the new password and no changes were made. Sorry for the inconvenience, try changing the password at a later date.";
                    return RedirectToAction("showChangePasswordView", "Profile");
                }
            }

            TempData["GeneralMessage"] = "Your password has been updated successfully.";
            TempData["CurrentPass"] = "";
            TempData["newPass"] = "";
            TempData["confirmPass"] = "";
            return RedirectToAction("showProfileView", "Profile");
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

            if (creditCard.number != "" && creditCard.date != "" && creditCard.ccv != 0)
            {
                if (!Regex.IsMatch(creditCard.number, @"^\d{16}$"))
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

            if ((creditCard.number == "" || creditCard.number == null) &&
                (creditCard.date == "" || creditCard.date == null) &&
                creditCard.ccv == 0)
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
            else
            {
                if (SQLHelper.SQLCheckEmail(user.email))
                    ModelState.AddModelError("email", "This email already in use, try a different one.");
            }

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
            {
                ViewBag.generalErrorMessage = "Updates to your profile information have been saved succesfully.";
                return RedirectToAction("showProfileView");
            }
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

            if ((user.creditCard.number == "" || user.creditCard.number == null ) &&
                (user.creditCard.date == "" || user.creditCard.date == null) &&
                user.creditCard.ccv == 0)
            {
                user.creditCard = null;
            }

            return Models.User.currentUser.updateInfo(user);
        }

        [HttpPost]
        public IActionResult DeleteCreditCard()
        {
            if (Models.User.currentUser.deleteCreditCard())
            {
                Models.User.currentUser.creditCard = null; //set the credit card to be null indicating that it has been deleted
                return Ok(); //return HTTP 200 OK status if successful
            }
            else
                return BadRequest(); //return badRequest indicating that we were unable to delete credit card
        }

        [HttpPost]
        public IActionResult DeleteAddress()
        {
            if (Models.User.currentUser.deleteAddress())
            {
                Models.User.currentUser.address = null; //set the address to be null indicating that it has been deleted
                return Ok(); //return HTTP 200 OK status if successful
            }
            else
                return BadRequest(); //return badRequest indicating that we were unable to delete address
        }

        private string saveCardNumberInViewBag()
        {
            string cardNumber = null;

            if (Models.User.currentUser != null && Models.User.currentUser.creditCard != null)
            {
                // Getting credit card encryption key for this user
                byte[] key = Encryption.getKeyFromFile(Models.User.currentUser.userId);
                if (key != null)
                {
                    cardNumber = Encryption.decryptAES(Models.User.currentUser.creditCard.number, key);
                    Array.Clear(key, 0, key.Length);
                    return cardNumber;
                }
            }

            return cardNumber;
        }
    }
}

