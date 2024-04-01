using System.Text.RegularExpressions;
using Stripe;

namespace BookHeaven.Models
{
    public class Address
    {
        public int userId {  get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public int apartNum { get; set; }

        public Address() { }

        public Address(int userId, string country, string city, string street, int apartNum)
        {
            this.userId = userId;
            this.country = country;
            this.city = city;
            this.street = street;
            this.apartNum = apartNum;
        }

        public static bool checkAddresses(Address a, Address b)
        {
            if (a.country == b.country && a.city == b.city && a.street == b.street && a.apartNum == b.apartNum)
                return true;
            else
                return false;
        }

        public static string addressValidation(Address address, User currentUser)
        {
            if (address.country != "" && address.city != "" && address.street != "" && address.apartNum != 0)
            {
                int flag = 0;
                string errorMessage = "";

                if (address.country == null || !Regex.IsMatch(address.country, @"^[a-zA-Z\s]{2,25}$"))
                {
                    errorMessage += "Invalid Country. ";
                    flag = 1;
                }

                if (address.city == null || !Regex.IsMatch(address.city, @"^[a-zA-Z\s]{2,25}$"))
                {
                    errorMessage += "Invalid City. ";
                    flag = 1;
                }

                if (address.street == null || !Regex.IsMatch(address.street, @"^[a-zA-Z\s]{2,25}$"))
                {
                    errorMessage += "Invalid Street. ";
                    flag = 1;
                }

                if (address.apartNum == 0)
                {
                    errorMessage += "Invalid Apartment Number. ";
                    flag = 1;
                }

                if (flag == 1)
                {
                    return errorMessage;
                }
                else
                {
                    address.userId = currentUser.userId;
                    return "valid";
                }
            }

            if (checkMissingValues(address))
                return "Please enter the address information.";
            else
            {
                address.userId = currentUser.userId;
                return "valid";
            }
        }

        public static bool checkIsEmpty(Address address)
        {
            if ((address.country == "" || address.country == null) &&
                (address.city == "" || address.city == null) &&
                (address.street == "" || address.street == null) &&
                address.apartNum == 0)
            {
                return true;
            }
            return false;
        }

        public static bool checkMissingValues(Address address)
        {
            if ((address.country == "" || address.country == null) ||
                (address.city == "" || address.city == null) ||
                (address.street == "" || address.street == null) ||
                address.apartNum == 0)
            {
                return true;
            }
            return false;
        }
    }
}
