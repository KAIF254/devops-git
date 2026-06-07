

namespace OnlineBookStore.Models
{
    public class OrderItem
    {
        // Id: Unique number for each order item
        public int Id { get; set; }

        // OrderId: Which order this item belongs to
        public int OrderId { get; set; }

        // BookId: Which book was purchased
        public int BookId { get; set; }

        // Quantity: How many copies of the book were purchased
        public int Quantity { get; set; }

        // Price: Price of the book at the time of purchase
        public decimal Price { get; set; }
    }
}
