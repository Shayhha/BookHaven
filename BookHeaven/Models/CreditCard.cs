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
    }
}
