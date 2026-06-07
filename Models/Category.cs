

namespace OnlineBookStore.Models
{
    public class Category
    {
        // Id: Unique number for each category (auto-generated)
        public int Id { get; set; }

        // Name: Category name, e.g. "Fiction", "Science", "History"
        public string Name { get; set; } = string.Empty;
    }
}
