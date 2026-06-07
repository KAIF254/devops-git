

namespace OnlineBookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        // Title: Title of the book
        public string Title { get; set; } = string.Empty;

        // Author: Name of the author
        public string Author { get; set; } = string.Empty;

        // Description: A short summary about the book
        public string Description { get; set; } = string.Empty;

        // Price: Cost of the book (e.g. 9.99)
        public decimal Price { get; set; }

        // Stock: How many copies are available
        public int Stock { get; set; }

        // CategoryId: Which category this book belongs to
        public int CategoryId { get; set; }

        // ReleaseDate: When the book was published
        public DateTime? ReleaseDate { get; set; }

        // ImageUrl: Optional cover image URL; if empty we use a gradient placeholder
        public string? ImageUrl { get; set; }
    }
}
