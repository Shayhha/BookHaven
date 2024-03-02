namespace BookHeaven.Models
{
    public class Book
    {
        public string name {  get; set; }
        public string author { get; set; }
        public string date { get; set; }
        public int bookId { get; set; }
        public string category { get; set; }
        public string format { get; set; }
        public int price { get; set; }
        public int stock { get; set; }
        public string imageUrl { get; set; }
        public int ageLimitation { get; set; }
        public int salePrice {  get; set; }

        public Book() { }

        public Book(string name, string author, string date, int bookId, string category, string format, int price, int stock, string imageUrl, int ageLimitation, int salePrice)
        {
            this.name = name;
            this.author = author;
            this.date = date;
            this.bookId = bookId;
            this.category = category;
            this.format = format;
            this.price = price;
            this.stock = stock;
            this.imageUrl = imageUrl;
            this.ageLimitation = ageLimitation;
            this.salePrice = salePrice;
        }
    }
}
