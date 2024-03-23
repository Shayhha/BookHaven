namespace BookHeaven.Models
{
    public class Order
    {
        public int orderId, userId;
        public string orderDate, shippingDate, shippingNum;
        public float totalPrice;
        public List<OrderItem> orderItems;

        public Order(int orderId, int userId, string orderDate, string shippingDate, string shippingNum, float totalPrice, List<OrderItem> orderItems)
        {
            this.orderId = orderId;
            this.userId = userId;
            this.orderDate = orderDate;
            this.shippingDate = shippingDate;
            this.shippingNum = shippingNum;
            this.totalPrice = totalPrice;
            this.orderItems = orderItems;
        }
    }
}
