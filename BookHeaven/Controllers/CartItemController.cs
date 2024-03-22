using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace BookHeaven.Controllers
{
    public class CartItemController : Controller
    {
        // this part is for the session variables
        private readonly IHttpContextAccessor _contx;

        public CartItemController(IHttpContextAccessor contx)
        {
            _contx = contx;
        }
        // # # #


        public IActionResult showCartView()
        {
            return View("CartView", Models.User.currentUser);
        }


        public IActionResult addBookToCart(int bookId)
        {
            bool success = false;
            string errorMessage = "";

            if (Models.User.currentUser.existsInCart(bookId))
            {
                errorMessage = "This book is already in your cart.";
                return Json(new { success = success, errorMessage = errorMessage });
            }

            Book book = SQLHelper.SQLSearchBookById(bookId);

            if (book != null)
            {
                if (book.stock == 0)
                {
                    errorMessage = "This book is sold out, sorry for your inconvenience.";
                    return Json(new { success = success, errorMessage = errorMessage });
                }

                CartItem cartItem = new CartItem(book, 1);

                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    success = Models.CartItem.addCartItem(Models.User.currentUser.userId, cartItem);
                }
                else
                {
                    success = Models.CartItem.addCartItem(cartItem);
                }

                if (success)
                    Console.WriteLine("The book '" + book.name + "' has been added to the cart");
            }

            return Json(new { success = success, errorMessage = errorMessage });
        }

        public IActionResult updateBookInCart(int bookId, int quantity)
        {
            bool success = false;
            Book book = SQLHelper.SQLSearchBookById(bookId);

            if (book != null)
            {
                CartItem cartItem = new CartItem(book, quantity);

                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    success = Models.CartItem.updateCartItem(Models.User.currentUser.userId, cartItem);
                }
                else
                {
                    success = Models.CartItem.updateCartItem(cartItem);
                }

                if (success)
                    Console.WriteLine("The book '" + book.name + "' has been updated in the cart");
            }

            return Json(new { success = success });
        }

        public IActionResult deleteBookFromCart(int bookId)
        {
            bool success = false;
            
            if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
            {
                success = Models.CartItem.deleteCartItem(Models.User.currentUser.userId, bookId);
            }
            else
            {
                success = Models.CartItem.deleteCartItem(bookId);
            }

            if (success)
                Console.WriteLine("The book number " + bookId + " has been deleted from the cart");
            
            return Json(new { success = success });
        }


        public IActionResult checkoutFromCart()
        {
            if (Models.User.currentUser != null && Models.User.currentUser.cartItems != null) {

                SessionCreateOptions options = new SessionCreateOptions
                {
                    SuccessUrl = "https://localhost:7212/CartItem/checkoutWasSuccessful",
                    CancelUrl = "https://localhost:7212/CartItem/checkoutHasFailed", 
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (CartItem cartItem in Models.User.currentUser.cartItems)
                {
                      options = Payment.addItemToCheckout(options, cartItem);
                }

                SessionService service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);

            }

            // Create a ViewBag message for the user
            return new StatusCodeResult(400); // Bad Request
        }


        public IActionResult checkoutWasSuccessful()
        {
            Models.User.currentUser.cartItems.Clear();
            // Create a ViewBag success message to the user
            return RedirectToAction("showUserHome", "UserHome");
        }

        public IActionResult checkoutHasFailed()
        {
            // Create a ViewBag failure message to the user
            return RedirectToAction("showCartView", "CartItem");
        }
    }
}

