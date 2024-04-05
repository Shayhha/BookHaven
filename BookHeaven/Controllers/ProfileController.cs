using System.Text.RegularExpressions;
using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookHeaven.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult showProfileView()
        {
            string message = TempData["GeneralMessage"] as string;
            ViewBag.GeneralMessage = message;
            Models.User.currentUser.orders = SQLHelper.SQLInitUserOrders(Models.User.currentUser.userId);
            ViewBag.cardNumber = Payment.saveCardNumberInViewBag();
            return View("ProfileView", Models.User.currentUser);
        }

        public IActionResult handleCancelButton()
        {
            ViewBag.cardNumber = Payment.saveCardNumberInViewBag();
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
            ViewBag.cardNumber = Payment.saveCardNumberInViewBag();
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


        public IActionResult CheckUsersInput(User user)
        {
            ViewBag.cardNumber = user.creditCard.number;

            ModelState.Clear();
            user.userId = Models.User.currentUser.userId;

            List<string> validationResults = Models.User.validateUserInfo(user);
            if (validationResults[0] != "valid") { // [0] is the type, for example email. [1] is the error message it self.
                ModelState.AddModelError(validationResults[0], validationResults[1]);
            }

            // Perform custom address validation
            if (!Address.checkIsEmpty(user.address))
            {
                if (!Address.checkMissingValues(user.address))
                {
                    string addressErrorMessage = Address.addressValidation(user.address, user);
                    if (addressErrorMessage != "valid")
                        ModelState.AddModelError("address", addressErrorMessage);
                }
                else
                    ModelState.AddModelError("address", "Fill in all address values");  
            }
            else
                user.address = null;
            

            // Perform custom credit card validation
            if (!CreditCard.checkIsEmpty(user.creditCard))
            {
                if (!CreditCard.checkMissingValues(user.creditCard))
                {
                    string creditCardErrorMessage = CreditCard.creditCardValidation(user.creditCard, user);
                    if (creditCardErrorMessage != "valid")
                    {
                        ModelState.AddModelError("creditCard", creditCardErrorMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError("creditCard", "Fill in all credit card values");
                }
            }
            else
                user.creditCard = null;
            

            if (!ModelState.IsValid)
                return View("EditProfileView", user);
            
            if (Models.User.currentUser.updateInfo(user))
            {
                TempData["GeneralMessage"] = "Updates to your profile information have been saved succesfully.";
                return RedirectToAction("showProfileView");
            }
            else
            {
                TempData["GeneralMessage"] = "Unable to save the changes, something went wrong please try again.";
                return View("EditProfileView", user);
            }
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
    }
}

