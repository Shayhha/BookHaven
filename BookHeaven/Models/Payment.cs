using Stripe.Checkout;

namespace BookHeaven.Models
{
    public class Payment
    {
        public Book? book { get; set; }
        public int quantity { get; set; }
        public CreditCard? creditCard { get; set; }
        public Address? address { get; set; }

        public Payment()
        {
            // Default constructor
        }

        public Payment(Book book, int quantity, CreditCard creditCard, Address address)
        {
            this.book = book;
            this.quantity = quantity;
            this.creditCard = creditCard;
            this.address = address;
        }

        public Payment(string name, string author, string bookDate, int bookId, string category, string format, float price, int stock, string imageUrl, int ageLimitation, int salePrice, int quantity, string number, string date, int ccv, string country, string city, string street, int apartNum)
        {
            this.quantity = quantity;
            this.book = new Book(name,author,bookDate, bookId,category,format,price,stock,imageUrl,ageLimitation,salePrice);
            this.creditCard = new CreditCard(Models.User.currentUser.userId, number, date, ccv);
            this.address = new Address(Models.User.currentUser.userId, country, city, street, apartNum);
        }


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

        public static string saveCardNumberInViewBag()
        {
            string cardNumber = null;

            if (User.currentUser != null && User.currentUser.creditCard != null)
            {
                // Getting credit card encryption key for this user
                byte[] key = Encryption.getKeyFromFile(User.currentUser.userId);
                if (key != null)
                {
                    cardNumber = Encryption.decryptAES(User.currentUser.creditCard.number, key);
                    Array.Clear(key, 0, key.Length);
                    return cardNumber;
                }
            }

            return cardNumber;
        }
    }
}
