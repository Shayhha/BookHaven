using System;
namespace BookHeaven.Models
{
	public class OrderItem
	{
		public string bookName;
        public int orderDetailId, orderId, quantity;
        public float price;

		public OrderItem(int orderDetailId, int orderId, string bookName, int quantity, float price)
		{
			this.orderDetailId = orderDetailId;
			this.orderId = orderId;
			this.bookName = bookName;
			this.quantity = quantity;
			this.price = price;
        }
	}
}

