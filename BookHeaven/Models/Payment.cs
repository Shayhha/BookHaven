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
    }
}
