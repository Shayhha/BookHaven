using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;
using System.Net;

namespace BookHeaven.Controllers
{
    public class BookController : Controller
    {
        public IActionResult showBookEdit(int bookId)
        {
            Book book = SQLHelper.SQLSearchBookById(bookId);
            if (book != null)
            {
                return View("BookEditView", book);
            }
            return View("UserHome/showUserHome");
        }

        public IActionResult updateBook(Book updatedBook)
        {
            // do some logic here
            return RedirectToAction("showUserHome", "UserHome");
        }
    }
}
