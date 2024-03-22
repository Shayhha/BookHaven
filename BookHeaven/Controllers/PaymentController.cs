using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult processPayment()
        {
            return View("PaymentView");
        }
    }
}
