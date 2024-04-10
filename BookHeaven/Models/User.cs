using System.Text.RegularExpressions;

namespace BookHeaven.Models
{
    public class User
    {
        public static User? currentUser = null; //this is current user that is logged into the website
        public int userId { get; set; }
        public string email { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }

        public Address? address { get; set; }
        public CreditCard? creditCard { get; set; }
        public List<CartItem>? cartItems { get; set; }
        public List<Order>? orders { get; set; }
        public bool isAdmin { get; set; }

        public User() {
            this.cartItems = new List<CartItem>();
        }

        public User(int userId, string email, string fname, string lname, bool isAdmin = false)
        {
            this.userId = userId;
            this.email = email;
            this.fname = fname;
            this.lname = lname;
            this.address = null;
            this.creditCard = null;
            this.cartItems = new List<CartItem>();
            this.orders = null;
            this.isAdmin = isAdmin;
        }


        public User(int userId, string email, string fname, string lname, string country, string city, string street, int apartNum, string number, string date, int ccv, bool isAdmin = false)
        {
            this.userId = userId;
            this.email = email;
            this.fname = fname;
            this.lname = lname;
            this.cartItems = new List<CartItem>();
            this.orders = null;
            this.isAdmin = isAdmin;
            if (country == "" && city == "" && street == "" && apartNum == 0)
                this.address = null;
            else
                this.address = new Address(userId, country, city, street, apartNum);

            if (number == "" && date == "" && ccv == 0)
                this.creditCard = new CreditCard(userId, number, date, ccv);
            else
                this.creditCard = null;
        }

        public void setCartItems(List<CartItem> cartItems)
        {
            if (cartItems != null)
            {
                this.cartItems = cartItems;
            }
        }

        public static bool checkUsers(User a, User b)
        {
            if (a.email == b.email && a.fname == b.fname && a.lname == b.lname && a.isAdmin == b.isAdmin)
                return true;
            else
                return false;
        }

        public static List<string> validateUserInfo(User user)
        {
            if (user.email == null || !Regex.IsMatch(user.email, @"^[\w-.]+@([\w-]+\.)+[\w-]{2,4}$"))
                return new List<string> { "email", "Email follow the general email template: example@abc.xyz" };
            else
            {
                if ((currentUser.email != user.email) && SQLHelper.SQLCheckEmail(user.email))
                    return new List<string> { "email", "This email already in use, try a different one." };
            }

            if (user.fname == null || !Regex.IsMatch(user.fname, @"^[a-zA-Z]{2,20}$"))
                return new List<string> { "fname", "First name should be only letters." };

            if (user.lname == null || !Regex.IsMatch(user.lname, @"^[a-zA-Z]{2,20}$"))
                return new List<string> { "lname", "Last name should be only letters." };

            return new List<string> {"valid"};
        }

        public void setUser(string email, string fname, string lname)
        {
            this.email = email;
            this.fname = fname;
            this.lname = lname;
        }

        /// <summary>
        /// Method for updating user info in database and in currentUser object
        /// </summary>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        public bool updateInfo(User updatedUser)
        {
            //set regular info for user
            if (checkUsers(this, updatedUser) == false)
            {
                if (SQLHelper.SQLUpdateUserInfo(updatedUser) == false) //call our SQL function for changing the regular info for user
                    return false;
                this.setUser(updatedUser.email, updatedUser.fname, updatedUser.lname);
            }

            //check the address
            if (updatedUser.address != null)
            {
                if (this.address != null)
                {
                    if (Address.checkAddresses(this.address, updatedUser.address) == false)
                    {
                        if (SQLHelper.SQLUpdateAddress(updatedUser.address) == false)
                            return false;
                    }
                }
                else
                {
                    if (SQLHelper.SQLAddAddress(updatedUser.address) == false)
                        return false;
                }
                this.address = updatedUser.address;
            }

            //check the credit card
            if (updatedUser.creditCard != null)
            {
                // Encrypting the updated credit card number
                byte[] key = Encryption.getKeyFromFile(this.userId);
                if (key != null)
                {
                    updatedUser.creditCard.number = Encryption.encryptAES(updatedUser.creditCard.number, key);
                    Array.Clear(key, 0, key.Length);

                    if (this.creditCard != null)
                    {
                        if (CreditCard.checkCreditCard(this.creditCard, updatedUser.creditCard) == false)
                        {
                            if (SQLHelper.SQLUpdateCreditCard(updatedUser.creditCard) == false)
                                return false;
                        }
                    }
                    else
                    {
                        if (SQLHelper.SQLAddCreditCard(updatedUser.creditCard) == false)
                            return false;
                    }

                }
                else
                    return false;

                this.creditCard = updatedUser.creditCard;
            }
            return true;
        }

        public bool deleteAddress()
        {
            if (this.address != null)
            {
                if (SQLHelper.SQLDeleteAddress(this.userId) == true) //means we successfully deleted the address
                {
                    this.address = null; //set the address in our currentUser object to be null indicating used doesn't have address anymore
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool deleteCreditCard()
        {
            if (this.creditCard != null)
            {
                if (SQLHelper.SQLDeleteCreditCard(this.userId) == true) //means we successfully deleted the credit card
                {
                    this.creditCard = null; //set the creditCard in our currentUser object to be null indicating used doesn't have credit card anymore
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool existsInCart(int bookId) // checks if the given bookId exists in the cart
        {
            if (this.cartItems != null)
            {
                foreach (CartItem item in this.cartItems)
                {
                    if (item.book != null && item.book.bookId == bookId)
                        return true;
                }
            }
            return false;
        }

        public void mergeCartLists(User currentDefaultUser)
        {
            if (this.cartItems != null && currentDefaultUser != null && currentDefaultUser.cartItems != null && currentDefaultUser.cartItems.Any())
            {
                if (!this.isAdmin)
                    this.cartItems = SQLHelper.SQLBulkInsertCartItems(this.userId, currentDefaultUser.cartItems);
            }
        }


    }
}
