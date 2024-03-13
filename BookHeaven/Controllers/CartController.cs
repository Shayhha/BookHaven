using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class CartController : Controller
    {
        public IActionResult showCartView()
        {
            return View("CartView");
        }

        public IActionResult addBookToCart(int bookId)
        {
            bool success = Models.Cart.addBookToCart(bookId);
            return Json(new { success = success });
        }
    }
}
