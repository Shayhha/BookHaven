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

        public IActionResult showAddNewBook()
        {
            return View("AddNewBookView");
        }

        public IActionResult addBook(Book newBook)
        {
            // do some logic here
            return RedirectToAction("showUserHome", "UserHome"); // this is temporary
        }

        public IActionResult updateBook(Book updatedBook)
        {
            // do some logic here
            return RedirectToAction("showUserHome", "UserHome"); // this is temporary
        }
    }
}
