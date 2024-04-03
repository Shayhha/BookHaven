using BookHeaven.Extensions;
using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe;
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

        public IActionResult openPaymentCartView()
        {
            Payment payment = HttpContext.Session.GetObjectFromJson<Payment>("PaymentObject");
            HttpContext.Session.Remove("PaymentObject");

            ViewBag.GeneralMessage = TempData["GeneralMessage"] as string;
            ViewBag.cardNumber = payment.creditCard.number;

            return View("PaymentCartView", payment);
        }

        public IActionResult showPaymentView(int bookId, int quantity)
        {
            if (Models.User.currentUser != null)
            {
                User tempUser = Models.User.currentUser;
                Book book = SQLHelper.SQLSearchBookById(bookId);
                if (book != null)
                {
                    if (book.stock == 0)
                    {
                        TempData["GeneralMessage"] = "The book you are tring to buy is currently out of stock, please try again later.";
                        return RedirectToAction("showUserHome", "UserHome");
                    }

                    Payment payment = new Payment(book, quantity, tempUser.creditCard, tempUser.address);

                    ViewBag.cardNumber = Payment.saveCardNumberInViewBag();
                    return View("PaymentView", payment);
                }
            }

            TempData["GeneralMessage"] = "An error has occured, please try again later.";
            return RedirectToAction("showUserHome", "UserHome");
        }

        public IActionResult showPaymentViewFromCart()
        {
            if (Models.User.currentUser != null)
            {
                User tempUser = Models.User.currentUser;
                string total = TempData["totalPayment"] as string;
                Payment payment = new Payment(null, 0, float.Parse(total), tempUser.creditCard, tempUser.address);
                ViewBag.cardNumber = Payment.saveCardNumberInViewBag();
                //ViewBag.totalPayment = TempData["totalPayment"] as string;
                return View("PaymentCartView", payment);
            }

            TempData["GeneralMessage"] = "An error has occured, please try again later.";
            return RedirectToAction("showUserHome", "UserHome");
        }

        public IActionResult returnToPaymentView(Payment payment, string errorMessage)
        {
            ViewBag.GeneralMessage = errorMessage;
            ViewBag.cardNumber = payment.creditCard.number;
            return View("PaymentView", payment);
        }


        public IActionResult processPayment(Payment payment)
        {
            if (payment.address != null)
            {
                string errorMessage = Models.Address.addressValidation(payment.address, Models.User.currentUser);
                if (errorMessage != "valid")
                    return returnToPaymentView(payment, errorMessage);
            }

            if (payment.creditCard != null)
            {
                string errorMessage = CreditCard.creditCardValidation(payment.creditCard, Models.User.currentUser);
                if (errorMessage != "valid")
                    return returnToPaymentView(payment, errorMessage);
            }

            if (SQLHelper.SQLUpdateBookStock(payment.book.bookId, (-1) * payment.quantity))
                return checkoutWasSuccessful(payment.book.bookId, payment.quantity);
            else
            {
                TempData["GeneralMessage"] = "The book you are tring to buy is now out of stock, please try again later.";
                return RedirectToAction("showUserHome", "UserHome");
            }
        }


        public IActionResult processPaymentWithStripe(int bookId, int quantity)
        {
            if (SQLHelper.SQLUpdateBookStock(bookId, (-1) * quantity))
            {
                if (Models.User.currentUser != null)
                {
                    User tempUser = Models.User.currentUser;

                    CustomerCreateOptions customerOptions = new CustomerCreateOptions
                    {
                        Name = tempUser.fname + " " + tempUser.lname,
                        Email = tempUser.email,
                        PaymentMethod = "pm_card_visa",
                    };

                    CustomerService customerService = new CustomerService();
                    Customer customer = customerService.Create(customerOptions);

                    SessionCreateOptions options = new SessionCreateOptions
                    {
                        SuccessUrl = "https://localhost:7212/Payment/checkoutWasSuccessful?bookId=" + bookId + "&quantity=" + quantity,
                        CancelUrl = "https://localhost:7212/Payment/checkoutHasFailed?bookId=" + bookId + "&quantity=" + quantity,
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        Customer = customer.Id,
                        PaymentMethodTypes = new List<string> { "card" },
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

            TempData["GeneralMessage"] = "You are trying to buy more books than we have in stock, try reducing the quantity.";
            return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
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
