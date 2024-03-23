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

        public static bool addCartItemDefault(CartItem cartItem) //for user without db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                if (SQLHelper.SQLUpdateBookStock(cartItem.book.bookId, (-1) * cartItem.amount)) //we need to remove amount from stock in db
                {
                    Models.User.currentUser.cartItems.Add(cartItem); //add the cartItem
                    return true;
                }
            }
            return false;
        }

        public static bool addCartItemUser(int userId, CartItem cartItem) //for user with db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                if (SQLHelper.SQLAddCartItem(userId, cartItem)) //if we successfully added item
                {
                    Models.User.currentUser.cartItems.Add(cartItem); //add the cartItem
                    return true;
                }
            }
            return false;
        }

        public static bool updateCartItemDefault(CartItem cartItem, int amountDifference) //for user without db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                for (int i = 0; i < Models.User.currentUser.cartItems.Count; i++)
                {
                    if (Models.User.currentUser.cartItems[i].Equals(cartItem)) //if true found the object we want to change amount
                    {
                        if (SQLHelper.SQLUpdateBookStock(cartItem.book.bookId, amountDifference)) //try to update item amount
                        {
                            Models.User.currentUser.cartItems[i].amount = cartItem.amount; //set new amount
                            return true;
                        }
                        else
                            break;
                    }
                }
            }
            return false;
        }

        public static bool updateCartItemUser(int userId, CartItem cartItem, int amountDifference) //for user with db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                for (int i = 0; i < Models.User.currentUser.cartItems.Count; i++)
                {
                    if (Models.User.currentUser.cartItems[i].Equals(cartItem)) //if true found the object we want to change amount
                    {
                        if (SQLHelper.SQLUpdateCartItem(userId, cartItem, amountDifference)) //try to update item amount
                        {
                            Models.User.currentUser.cartItems[i].amount = cartItem.amount; //set new amount
                            return true;
                        }
                        else
                            break;
                    }
                }
            }
            return false;
        }

        public static bool deleteCartItemDefault(CartItem cartItem) //for users without db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                if (SQLHelper.SQLUpdateBookStock(cartItem.book.bookId, cartItem.amount)) //we need to add amount to stock
                {
                    Models.User.currentUser.cartItems.RemoveAll(cartItems => cartItems.book.bookId == cartItem.book.bookId); //remove item from cart list that matches the bookId
                    return true;
                }
            }
            return false;
        }

        public static bool deleteCartItemUser(int userId, CartItem cartItem) //for users with db
        {
            if (Models.User.currentUser.cartItems != null)
            {
                if (SQLHelper.SQLDeleteCartItem(userId, cartItem))
                {
                    Models.User.currentUser.cartItems.RemoveAll(cartItems => cartItems.book.bookId == cartItem.book.bookId); //remove item from cart list that matches the bookId
                    return true;
                }
            }
            return false;
        }

    }
}
