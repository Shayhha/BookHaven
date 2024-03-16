using System.ComponentModel.DataAnnotations;

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
        public bool isAdmin { get; set; }

        public User() { }

        public User(int userId, string email, string fname, string lname, bool isAdmin = false)
        {
            this.userId = userId;
            this.email = email;
            this.fname = fname;
            this.lname = lname;
            this.address = null;
            this.creditCard = null;
            this.cartItems = new List<CartItem>();
            this.isAdmin = isAdmin;
        }


        public User(int userId, string email, string fname, string lname, string country, string city, string street, int apartNum, long number, string date, int ccv, bool isAdmin = false)
        {
            this.userId = userId;
            this.email = email;
            this.fname = fname;
            this.lname = lname;
            this.cartItems = new List<CartItem>();
            this.isAdmin = isAdmin;
            if (country == "" && city == "" && street == "" && apartNum == 0)
                this.address = null;
            else
                this.address = new Address(userId, country, city, street, apartNum);

            if (number == 0 && date == "" && ccv == 0)
                this.creditCard = new CreditCard(userId, number, date, ccv);
            else
                this.creditCard = null;
        }

        public void setCartItems(List<CartItem> cartItems)
        {
            if(cartItems != null) 
            {
                this.cartItems = cartItems;
            }
        }

        public static bool checkUsers(User a, User b)
        {
            if(a.email == b.email && a.fname == b.fname && a.lname == b.lname && a.isAdmin == b.isAdmin)
                return true;
            else
                return false;
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
    }
}
