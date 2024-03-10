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
                return View("PaymentView", book);
            }
            return View("UserHome/showUserHome");
        }

        public IActionResult processPayment()
        {
            return View("PaymentView");
        }
    }
}
