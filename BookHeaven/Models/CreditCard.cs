using System.Text.RegularExpressions;
using Stripe;

namespace BookHeaven.Models
{
    public class CreditCard
    {
        public int userId { get; set; }
        public string number { get; set; }  
        public string date { get; set; }
        public int ccv { get; set; }

        public CreditCard() { }
        public CreditCard(int userId, string number, string date, int ccv)
        {
            this.userId = userId;
            this.number = number;
            this.date = date;
            this.ccv = ccv;
        }

        public static bool checkCreditCard(CreditCard a, CreditCard b)
        {
            if (a.number == b.number && a.date == b.date && a.ccv == b.ccv)
                return true;
            else
                return false;
        }

        public static string creditCardValidation(CreditCard creditCard, User currentUser)
        {
            int flag = 0;
            string errorMessage = "";

            if (creditCard.number != "" && creditCard.date != "" && creditCard.ccv != 0)
            {
                if (!Regex.IsMatch(creditCard.number, @"^\d{16}$"))
                {
                    errorMessage += "Invalid Card Number. ";
                    flag = 1;
                }

                if (!Regex.IsMatch(creditCard.date, @"^(0[1-9]|1[0-2])\/\d{2}$"))

                {
                    errorMessage += "Invalid Experation Date. ";
                    flag = 1;
                }

                if (!Regex.IsMatch(creditCard.ccv.ToString(), @"^[1-9]\d{2}$"))
                {
                    errorMessage += "Invalid CCV Number. ";
                    flag = 1;
                }

                if (flag == 1)
                {
                    return errorMessage;
                }
                else
                {
                    creditCard.userId = currentUser.userId;
                    return "valid";
                }
            }

            if (checkMissingValues(creditCard))
                return "Please enter the credit card information.";
            else
            {
                creditCard.userId = currentUser.userId;
                return "valid";
            }
        }

        public static bool checkIsEmpty(CreditCard creditCard)
        {
            if ((creditCard.number == "" || creditCard.number == null) &&
                (creditCard.date == "" || creditCard.date == null) &&
                creditCard.ccv == 0)
            {
                return true;
            }
            return false;
        }

        public static bool checkMissingValues(CreditCard creditCard)
        {
            if ((creditCard.number == "" || creditCard.number == null) ||
                (creditCard.date == "" || creditCard.date == null) ||
                creditCard.ccv == 0)
            {
                return true;
            }
            return false;
        }
    }
}
