﻿using BookHeaven.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookHeaven.Controllers
{
    public class CartItemController : Controller
    {
        // this part is for the session variables
        private readonly IHttpContextAccessor _contx;

        public CartItemController(IHttpContextAccessor contx)
        {
            _contx = contx;
        }
        // # # #


        public IActionResult showCartView()
        {
            return View("CartView", Models.User.currentUser);
        }


        public IActionResult addBookToCart(int bookId)
        {
            bool success = false;
            Book book = SQLHelper.SQLSearchBookById(bookId);

            if (book != null)
            {
                CartItem cartItem = new CartItem(book, 1);

                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    success = Models.CartItem.addCartItem(Models.User.currentUser.userId, cartItem);
                }
                else
                {
                    success = Models.CartItem.addCartItem(cartItem);
                }

                if (success)
                    Console.WriteLine("The book '" + book.name + "' has been added to the cart");
            }

            return Json(new { success = success });
        }

        public IActionResult updateBookInCart(int bookId, int quantity)
        {
            bool success = false;
            Book book = SQLHelper.SQLSearchBookById(bookId);

            if (book != null)
            {
                CartItem cartItem = new CartItem(book, quantity);

                if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
                {
                    success = Models.CartItem.updateCartItem(Models.User.currentUser.userId, cartItem);
                }
                else
                {
                    success = Models.CartItem.updateCartItem(cartItem);
                }

                if (success)
                    Console.WriteLine("The book '" + book.name + "' has been updated in the cart");
            }

            return Json(new { success = success });
        }

        public IActionResult deleteBookFromCart(int bookId)
        {
            bool success = false;
            
            if (_contx.HttpContext.Session.GetString("isLoggedIn") == "true")
            {
                success = Models.CartItem.deleteCartItem(Models.User.currentUser.userId, bookId);
            }
            else
            {
                success = Models.CartItem.deleteCartItem(bookId);
            }

            if (success)
                Console.WriteLine("The book number " + bookId + " has been deleted from the cart");
            
            return Json(new { success = success });
        }
    }
}
