﻿using Microsoft.AspNetCore.Mvc;
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
                if (Models.Book.checkBook(newBook)) //check if given book already exists
                {
                    ViewBag.errorMessage = "Book already exists.";
                    return View("AddNewBookView", newBook);
                }
                if (Models.Book.addBook(newBook)) //add the book to database 
                {
                    Console.WriteLine("The book " + newBook.name + " has been added to the website.");
                    return RedirectToAction("showUserHome", "UserHome");
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
                if (Models.Book.updateBook(updatedBook)) //update the book to database 
                {
                    Console.WriteLine("The book with id = " + updatedBook.bookId + " has been updated.");
                    //SQLHelper.SQLUpdateBookStock(updatedBook.bookId, 25, true);
                    return RedirectToAction("showUserHome", "UserHome");
                }
                else
                {
                    ViewBag.errorMessage = "Unable to update book, try again later.";
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
                if (Models.Book.deleteBook(bookId)) //add the book to database 
                {
                    Console.WriteLine("The book with id = " + bookId + " has been deleted form the website.");
                    return RedirectToAction("showUserHome", "UserHome");
                }
                else
                {
                    ViewBag.errorMessage = "Unable to remove book, try again later.";
                    return RedirectToAction("showUserHome", "UserHome");
                }
            }
            else
            {
                return RedirectToAction("showUserHome", "UserHome");
            }
        }
    }
}
