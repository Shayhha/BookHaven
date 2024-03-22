using Stripe.Checkout;

namespace BookHeaven.Models
{
    public class Payment
    {
        public static SessionCreateOptions addItemToCheckout(SessionCreateOptions options, CartItem cartItem)
        {
            if (cartItem != null && cartItem.book != null)
            {
                decimal itemPrice = Convert.ToDecimal(cartItem.book.salePrice > 0 ? cartItem.book.salePrice : cartItem.book.price);
                int itemAmount = cartItem.amount;
                string itemName = cartItem.book.name;
                List<string> imagesList = new List<string> { cartItem.book.imageUrl };

                SessionLineItemOptions sessionListItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = itemPrice * 100,
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = itemName,
                            Images = imagesList
                        }
                    },
                    Quantity = itemAmount
                };
                options.LineItems.Add(sessionListItem);
            }
            return options;
        }

        public static bool AddOrder(int userId, List<CartItem> cartItems)
        {
            if(cartItems != null)
            {
                DateTime currentDate = DateTime.Now; //represents current date
                string orderDate = currentDate.ToString("dd/MM/yyyy");//represents order date 
                string shippingDate = currentDate.AddDays(14).ToString("dd/MM/yyyy"); //represents shipping date
                
                float totalOrderPrice = 0;
                foreach(CartItem cartItem in cartItems) //iterate over the items in list to calculate total price
                {
                    float bookPrice = cartItem.book.salePrice > 0 ? cartItem.book.salePrice : cartItem.book.price;
                    totalOrderPrice += cartItem.amount * bookPrice;
                }
                if (SQLHelper.SQLAddOrder(userId, cartItems, orderDate, totalOrderPrice, shippingDate)) //if true means we added order to db
                    return true;
            }
            return false;
        }
    }
}
