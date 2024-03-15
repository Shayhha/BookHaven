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
            Book book = new Book();
            return View("AddNewBookView", book);
        }

        public IActionResult addBook(Book newBook)
        {
            if (ModelState.IsValid)
            {
                if (Models.Book.addBook(newBook)) //add the book to database 
                {
                    Console.WriteLine("The book " + newBook.name + " has been added to the website.");
                    return RedirectToAction("showUserHome", "UserHome");
                }
                else 
                {
                    ViewBag.errorMessage = "Book already exists.";
                    return View("AddNewBookView", newBook);
                }
            }
            else
            {
                Console.WriteLine("No");
                return View("AddNewBookView", newBook);
            }
        }

        public IActionResult updateBook(Book updatedBook)
        {
            // some SQL logic to update a book in the database.
            Console.WriteLine("The book " + updatedBook.name + " has been updated.");
            return RedirectToAction("showUserHome", "UserHome"); // this is temporary?
        }

        public IActionResult deleteBook(int bookId)
        {
            // some SQL logic to delete the book form the database.
            Console.WriteLine("The book with id = " + bookId + " has been deleted form the website.");
            return RedirectToAction("showUserHome", "UserHome"); 
        }
    }
}
