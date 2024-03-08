using Microsoft.AspNetCore.Mvc;

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

        public User() { }

        public User(int userId, string email, string fname, string lname)
        {
            this.userId = userId;
            this.email = email;
            this.fname = fname;
            this.lname = lname;
            this.address = null;
            this.creditCard = null;
        }

        //public bool updateInfo() { 
            
        //}

    }
}
