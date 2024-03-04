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
    }
}
