using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BookHeaven.Controllers
{
    public class PaymentController : Controller
    {
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
            if (Models.User.currentUser != null)
            {

                SessionCreateOptions options = new SessionCreateOptions
                {
                    SuccessUrl = "https://localhost:7212/Payment/checkoutWasSuccessful?bookId=" + bookId,
                    CancelUrl = "https://localhost:7212/Payment/checkoutHasFailed?bookId=" + bookId,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                CartItem cartItem = new CartItem(quantity); // Change this with a counter in the HTML
                cartItem.book = SQLHelper.SQLSearchBookById(bookId);

                options = Payment.addItemToCheckout(options, cartItem);
                

                SessionService service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);

            }

            // Create a ViewBag message for the user
            return new StatusCodeResult(400); // Bad Request
        }

        public IActionResult checkoutWasSuccessful(int bookId)
        {
            SQLHelper.SQLUpdateBookStock(bookId, 1); // Change the amount to the selected amount on the screen
            // Create a ViewBag success message to the user
            return RedirectToAction("showUserHome", "UserHome");
        }

        public IActionResult checkoutHasFailed(int bookId)
        {
            // Create a ViewBag failure message to the user
            return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
        }

    }
}
