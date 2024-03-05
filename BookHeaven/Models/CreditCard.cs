namespace BookHeaven.Models
{
    public class CreditCard
    {
        public int userId { get; set; }
        public long number { get; set; }
        public string date { get; set; }
        public int ccv { get; set; }

        public CreditCard() { }
        public CreditCard(int userId, long number, string date, int ccv)
        {
            this.userId = userId;
            this.number = number;
            this.date = date;
            this.ccv = ccv;
        }

    }
}
