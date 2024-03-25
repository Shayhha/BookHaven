using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;
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


        public IActionResult addBookToCart(int bookId, int quantity)
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
                else if (quantity > book.stock)
                {
                    errorMessage = "You are trying to buy more books than we have in stock, try reducing the quantity.";
                    return Json(new { success = success, errorMessage = errorMessage });
                }

                CartItem cartItem = new CartItem(book, quantity);

                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    success = Models.CartItem.addCartItemUser(Models.User.currentUser.userId, cartItem);
                }
                else
                {
                    success = Models.CartItem.addCartItemDefault(cartItem);
                }

                if (success)
                    Console.WriteLine("The book '" + book.name + "' has been added to the cart");
                else
                    errorMessage = "Cound not add the book to cart because we dont have enough in stock. try reducing the quantity.";

            }

            return Json(new { success = success, errorMessage = errorMessage });
        }

        public IActionResult updateBookInCart(int bookId, int newQuantity, int oldQuantity)
        {
            Console.WriteLine("new quantity = " + newQuantity + ", old quantity = " + oldQuantity);

            bool success = false;
            Book book = SQLHelper.SQLSearchBookById(bookId);

            if (book != null)
            {
                CartItem cartItem = new CartItem(book, newQuantity);

                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    success = Models.CartItem.updateCartItemUser(Models.User.currentUser.userId, cartItem, oldQuantity - newQuantity);
                }
                else
                {
                    success = Models.CartItem.updateCartItemDefault(cartItem, oldQuantity - newQuantity);
                }

                if (success)
                    Console.WriteLine("The book '" + book.name + "' has been updated in the cart");
            }

            return Json(new { success = success });
        }

        public IActionResult deleteBookFromCart(int bookId, int amount)
        {
            bool success = false;
            Book book = SQLHelper.SQLSearchBookById(bookId);

            if (book != null)
            {
                CartItem cartItem = new CartItem(book, amount);
                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    success = Models.CartItem.deleteCartItemUser(Models.User.currentUser.userId, cartItem);
                }
                else
                {
                    success = Models.CartItem.deleteCartItemDefault(cartItem);
                }
            }
            if (success)
                Console.WriteLine("The book number " + bookId + " has been deleted from the cart");
            
            return Json(new { success = success });
        }


        public IActionResult checkoutFromCart()
        {
            if (Models.User.currentUser != null && Models.User.currentUser.cartItems != null)
            {
                SessionCreateOptions options = new SessionCreateOptions
                {
                    SuccessUrl = "https://localhost:7212/CartItem/checkoutWasSuccessful",
                    CancelUrl = "https://localhost:7212/CartItem/checkoutHasFailed", 
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    CustomerEmail = Models.User.currentUser.email
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
            User currentUser = Models.User.currentUser;
            string message = "Could not save your order details. Please contact customer support for further assistance.";

            if (currentUser != null && currentUser.cartItems != null)
            {
                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    if (Payment.AddOrder(currentUser.userId, currentUser.cartItems))
                    {
                        if (SQLHelper.SQLDeleteUserCart(currentUser.userId))
                        {
                            currentUser.cartItems.Clear();
                            message = "Your payment was processed successfully! You can view your orders in your profile.";
                        }
                        else
                            message = "Your payment was processed successfully. But there was an error when updating your cart. \nPlease contant customer support.";
                    }
                }
                else
                {
                    currentUser.cartItems.Clear();
                    message = "Your payment was processed successfully! Check your email for the details.";
                }
            }

            TempData["GeneralMessage"] = message;
            return RedirectToAction("showUserHome", "UserHome"); 
        }

        public IActionResult checkoutHasFailed()
        {
            TempData["GeneralMessage"] = "Could not process your payment. Try again later.";
            return RedirectToAction("showCartView", "CartItem");
        }


        /// <summary>
        /// This method only runs when the user redirects to a different site. What it does it clear the default user's cart
        /// and update the stock of each item in the database according to the amount of each item that was in the user's cart.
        /// It does not affect the regular logged-in users in the application.
        /// </summary>
        /// <returns>Success message to the JavaScript method that calls this function, located in site.js at the bottom.</returns>
        public IActionResult clearDefaultUserCart()
        {
            if (Models.User.currentUser != null && Models.User.currentUser.cartItems != null &&
                Models.User.currentUser.cartItems.Count != 0)
            {
                string isLoggedIn = _contx.HttpContext.Session.GetString("isLoggedIn");
                if (string.IsNullOrEmpty(isLoggedIn) || isLoggedIn != "true")
                {
                    foreach (CartItem cartItem in Models.User.currentUser.cartItems)
                    {
                        if (cartItem.book != null)
                            SQLHelper.SQLUpdateBookStock(cartItem.book.bookId, cartItem.amount);
                    }
                    Models.User.currentUser.cartItems = new List<CartItem>();
                }
            }
            return Json(new { success = true });
        }
    }
}

