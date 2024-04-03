using Microsoft.AspNetCore.Mvc;
using BookHeaven.Models;

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
            return RedirectToAction("showUserHome", "UserHome");
        }

        public IActionResult showAddNewBook()
        {
            Book book = new Book();
            return View("AddNewBookView", book);
        }

        public IActionResult showBookInfoView(int bookId)
        {
            string message = TempData["GeneralMessage"] as string;
            Book book = SQLHelper.SQLSearchBookById(bookId);
            if (book != null)
            {
                ViewBag.addToCartSuccess = TempData["addToCartSuccess"];
                ViewBag.GeneralMessage = message;
                return View("BookInfoView", book);
            }
            return RedirectToAction("showUserHome", "UserHome");
        }

        public IActionResult addBook(Book newBook)
        {
            if (ModelState.IsValid)
            {
                if (Book.checkBook(newBook)) //check if given book already exists
                {
                    ViewBag.errorMessage = "The book you are trying to add already exists. Try a different Name, Author or Date.";
                    return View("AddNewBookView", newBook);
                }
                if (Book.addBook(newBook)) //add the book to database 
                {
                    TempData["GeneralMessage"] = "The book " + newBook.name + " has been successfully added to the website.";
                    return RedirectToAction("showAdminHome", "AdminHome");
                }
                else 
                {
                    ViewBag.errorMessage = "Unable to add book, try again later.";
                    return View("AddNewBookView", newBook);
                }
            }
            else
            {
                return View("AddNewBookView", newBook);
            }
        }

        public IActionResult updateBook(Book updatedBook)
        {
            if (ModelState.IsValid)
            {
                // find the book and see if the name, date or author has changed
                Book oldBookInfo = SQLHelper.SQLSearchBookById(updatedBook.bookId);
                if (!oldBookInfo.Equals(updatedBook))
                {
                    if (SQLHelper.SQLCheckBook(updatedBook.name, updatedBook.author, updatedBook.date))
                    {
                        ViewBag.errorMessage = "Unable to update the book " + oldBookInfo.name + " because there exist a book with the same Name, Author and Date.";
                        return View("BookEditView", oldBookInfo);
                    }
                }
                
                if (Book.updateBook(updatedBook)) //update the book to database 
                {
                    TempData["GeneralMessage"] = "The book " + updatedBook.name + " has been successfully updated.";
                    return RedirectToAction("showAdminHome", "AdminHome");
                }
                else
                {
                    ViewBag.errorMessage = "Unable to update the book " + updatedBook.name + ", try again later.";
                    return View("BookEditView", updatedBook);
                }
            }
            else
            {
                return View("BookEditView", updatedBook);
            }
        }

        public IActionResult deleteBook(int bookId)
        {
            if (ModelState.IsValid)
            {
                if (Book.deleteBook(bookId)) //delete book from database 
                {
                    TempData["GeneralMessage"] = "The book with id = " + bookId + " has been successfully deleted form the website.";
                    return RedirectToAction("showAdminHome", "AdminHome");
                }
                else
                {
                    ViewBag.errorMessage = "Unable to remove book with id = " + bookId + ", try again later.";
                    return RedirectToAction("showAdminHome", "AdminHome");
                }
            }
            else
            {
                return RedirectToAction("showAdminHome", "AdminHome");
            }
        }


        public IActionResult restockBook(int bookId, int restockAmount)
        {
            if (Book.updateBookStock(bookId, restockAmount))
            {
                TempData["GeneralMessage"] = "The book with id = " + bookId + " has been successfully restocked.";
                return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
            }
            else
            {
                TempData["GeneralMessage"] = "Unable to restock the book with id = " + bookId + " ..., try again later.";
                return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
            }
        }

        public IActionResult putBookOnSale(int bookId, float salePrice)
        {
            if (Book.updateBookPrice(bookId, salePrice, true))
            {
                TempData["GeneralMessage"] = "The book with id = " + bookId + " is on sale.";
                return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
            }
            else
            {
                TempData["GeneralMessage"] = "Unable to put the book with id = " + bookId + " on sale ..., try again later.";
                return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
            }
        }

        public IActionResult removeBookFromSale(int bookId)
        {
            if (Book.updateBookPrice(bookId, 0, true))
            {
                TempData["GeneralMessage"] = "The book with id = " + bookId + " is no longer on sale.";
                return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
            }
            else
            {
                TempData["GeneralMessage"] = "Unable to remove the book with id = " + bookId + " from sale ..., try again later.";
                return RedirectToAction("showBookInfoView", "Book", new { bookId = bookId });
            }
        }
    }
}
