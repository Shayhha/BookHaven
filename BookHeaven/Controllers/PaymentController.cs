using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BookHeaven.Controllers
{
    public class PaymentController : Controller
    {
        // this part is for the session variables
        private readonly IHttpContextAccessor _contx;

        public PaymentController(IHttpContextAccessor contx)
        {
            _contx = contx;
        }
        // # # #

        public IActionResult showPaymentView(int bookId)
        {
            Book book = SQLHelper.SQLSearchBookById(bookId);
            if (book != null)
            {
                if (book.stock == 0)
                {
                    // Create and show a viewbag error message that the book is not in stock
                    return RedirectToAction("showUserHome", "UserHome");
                }
                return View("PaymentView", book);
            }
            return RedirectToAction("showUserHome", "UserHome");

        }

        public IActionResult processPayment(int bookId, int quantity)
        {
            if (SQLHelper.SQLUpdateBookStock(bookId, (-1) * quantity))
            {
                if (Models.User.currentUser != null)
                {
                    SessionCreateOptions options = new SessionCreateOptions
                    {
                        SuccessUrl = "https://localhost:7212/Payment/checkoutWasSuccessful?bookId=" + bookId + "&quantity=" + quantity,
                        CancelUrl = "https://localhost:7212/Payment/checkoutHasFailed?bookId=" + bookId + "&quantity=" + quantity,
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        CustomerEmail = Models.User.currentUser.email,
                    };

                    CartItem cartItem = new CartItem(quantity); // Change this with a counter in the HTML
                    cartItem.book = SQLHelper.SQLSearchBookById(bookId);

                    options = Payment.addItemToCheckout(options, cartItem);

                    SessionService service = new SessionService();
                    Session session = service.Create(options);

                    Response.Headers.Add("Location", session.Url);

                    return new StatusCodeResult(303);

                }
            }

            // Create a ViewBag message for the user
            return new StatusCodeResult(400); // Bad Request
        }

        public IActionResult checkoutWasSuccessful(int bookId, int quantity)
        {

            User currentUser = Models.User.currentUser;
            string message = "Could not save your order details. Please contact customer support for further assistance.";

            if (currentUser != null)
            {
                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    List<CartItem> tempList = new List<CartItem> { new CartItem(SQLHelper.SQLSearchBookById(bookId), quantity) };
                    if (Payment.AddOrder(currentUser.userId, tempList))
                        message = "Your payment was processed successfully! You can view your orders in your profile.";
                   
                }
                else
                    message = "Your payment was processed successfully! Check your email for the details.";
            }

            TempData["GeneralMessage"] = message;
            return RedirectToAction("showUserHome", "UserHome");
        }

        public IActionResult checkoutHasFailed(int bookId, int quantity)
        {
            User currentUser = Models.User.currentUser;
            string message = "Your order is canceled.";

            if (currentUser != null)
            {
                if (!SQLHelper.SQLUpdateBookStock(bookId, quantity))
                    message = "Your order is canceled. But there was a problem updating the book stock.";
            }

            TempData["GeneralMessage"] = message;
            return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
        }

    }
}
