

namespace OnlineBookStore.Models
{
    public class CartItem
    {
        // BookId: which book was added
        public int BookId { get; set; }

        // Quantity: how many copies the user wants to buy
        public int Quantity { get; set; }
    }
}
