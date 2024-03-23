using System;
namespace BookHeaven.Models
{
	public class OrderItem
	{
		public string bookName;
        public int orderDetailId, orderId, bookId, quantity;
        public float price;

		public OrderItem(int orderDetailId, int orderId, int bookId, string bookName, int quantity, float price)
		{
			this.orderDetailId = orderDetailId;
			this.orderId = orderId;
			this.bookId = bookId;
			this.bookName = bookName;
			this.quantity = quantity;
			this.price = price;
        }
	}
}

