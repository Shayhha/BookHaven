using System.Net;

namespace BookHeaven.Models
{
    public class CartItem
    {
        public Book? book;
        public int amount;

        public CartItem() { }

        public CartItem(Book _book, int _amount)
        {
            book = _book;
            amount = _amount;
        }

        public CartItem(int _amount)
        {
            book = null;
            amount = _amount;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || this.GetType() != obj.GetType()) //check if they are same type
                return false;

            CartItem other = (CartItem)obj; //save obj as cartItem obj for reference
            if(this.book != null && other.book != null) //check if they're not null
                return this.book.bookId == other.book.bookId; //true if bookId is same

            return false; //return false in other scenarios
        }

        public static bool addCartItem(CartItem cartItem) //for user without db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                Models.User.currentUser.cartItems.Add(cartItem); //add the cartItem
                return true;
            }
            else
                return false;
        }

        public static bool addCartItem(int userId, CartItem cartItem) //for user with db
        {
            if (SQLHelper.SQLAddCartItem(userId, cartItem))
            {
                addCartItem(cartItem); //add cartItem to list
                return true;
            }
            else
                return false;
        }

        public static bool updateCartItem(CartItem cartItem) //for user without db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                for (int i = 0; i < Models.User.currentUser.cartItems.Count; i++)
                {
                    if (Models.User.currentUser.cartItems[i].Equals(cartItem)) //if true found the object we want to change amount
                        Models.User.currentUser.cartItems[i].amount = cartItem.amount; //set new amount
                }
                return true;
            }
            return false;
        }

        public static bool updateCartItem(int userId, CartItem cartItem) //for user with db
        {
            if (SQLHelper.SQLUpdateCartItem(userId, cartItem))
            {
                if(updateCartItem(cartItem))
                    return true;
                return false;
            }
            else
                return false;
        }

        public static bool deleteCartItem(int bookId) //for users without db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                Models.User.currentUser.cartItems.RemoveAll(cartItem => cartItem.book.bookId == bookId); //remove item from cart list that matches the bookId
                return true;
            }
            return false;
        }

        public static bool deleteCartItem(int userId, int bookId) //for users with db
        {
            if (SQLHelper.SQLDeleteCartItem(userId, bookId))
            {
                if (deleteCartItem(bookId))
                    return true;
                return false;
            }
            else
                return false;
        }
    }
}
