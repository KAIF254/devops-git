

namespace OnlineBookStore.Models
{
    public class Order
    {
        // Id: Unique order number (auto-generated)
        public int Id { get; set; }

        // UserId: Which user placed this order
        public int UserId { get; set; }

        // OrderDate: When the order was placed
        public DateTime OrderDate { get; set; }

        // TotalAmount: Total price of all books in this order
        public decimal TotalAmount { get; set; }

        // Status: Current order state — Pending / Shipped / Delivered / Cancelled
        public string Status { get; set; } = "Pending";
    }
}
