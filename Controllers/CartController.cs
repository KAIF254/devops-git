// ============================================================
// FILE: Controllers/CartController.cs
// PURPOSE: Manages the shopping cart stored in session memory.
//
//   Cart is saved as JSON in the session under key "Cart".
//   It holds a List<CartItem> (BookId + Quantity).
//
//   Actions:
//   - AddToCart      (POST) - adds a book, updates CartCount in session
//   - RemoveFromCart (POST) - removes a book from cart
//   - UpdateQuantity (POST) - updates qty for one book
//   - Index          (GET)  - shows the cart page
//   - Checkout       (GET)  - shows order summary before confirming
//   - PlaceOrder     (POST) - creates Order + OrderItems in DB, clears cart
// ============================================================

using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Data;
using OnlineBookStore.Models;
using System.Text.Json;

namespace OnlineBookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(json)) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            HttpContext.Session.SetInt32("CartCount", cart.Sum(x => x.Quantity));
        }

        // GET: /Cart
        
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["ErrorMessage"] = "Please login to view your cart.";
                return RedirectToAction("Login", "Account");
            }

            var cart    = GetCart();
            var bookIds = cart.Select(c => c.BookId).ToList();
            var books   = _context.Books.Where(b => bookIds.Contains(b.Id)).ToList();

            ViewBag.Books = books;
            return View(cart);
        }

        // POST: /Cart/AddToCart

        [HttpPost]
        public IActionResult AddToCart(int bookId, int quantity = 1)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["ErrorMessage"] = "Please login to add books to your cart.";
                return RedirectToAction("Login", "Account");
            }

            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                TempData["ErrorMessage"] = "Book not found.";
                return RedirectToAction("Index", "Books");
            }

            if (book.Stock <= 0)
            {
                TempData["ErrorMessage"] = "Sorry, this book is out of stock.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var cart     = GetCart();
            var existing = cart.FirstOrDefault(c => c.BookId == bookId);

            if (existing != null)
            {
            
                existing.Quantity = Math.Min(existing.Quantity + quantity, book.Stock);
            }
            else
            {
                cart.Add(new CartItem { BookId = bookId, Quantity = quantity });
            }

            SaveCart(cart);
            TempData["SuccessMessage"] = book.Title + " has been added to your cart!";
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        // POST: /Cart/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int bookId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.BookId == bookId);
            SaveCart(cart);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity
        
        [HttpPost]
        public IActionResult UpdateQuantity(int bookId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.BookId == bookId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    
                    cart.Remove(item);
                }
                else
                {
                    var book     = _context.Books.FirstOrDefault(b => b.Id == bookId);
                    int maxStock = book?.Stock ?? quantity;
                    item.Quantity = Math.Min(quantity, maxStock);
                }
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // GET: /Cart/Checkout
        public IActionResult Checkout()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["ErrorMessage"] = "Please login to checkout.";
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCart();

            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var bookIds = cart.Select(c => c.BookId).ToList();
            var books   = _context.Books.Where(b => bookIds.Contains(b.Id)).ToList();

            ViewBag.Books = books;
            return View(cart);
        }

        // POST: /Cart/PlaceOrder
        
        [HttpPost]
        public IActionResult PlaceOrder()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Please login to place an order.";
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCart();
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            var bookIds    = cart.Select(c => c.BookId).ToList();
            var books      = _context.Books.Where(b => bookIds.Contains(b.Id)).ToList();
            decimal total  = 0;

            foreach (var item in cart)
            {
                var book = books.FirstOrDefault(b => b.Id == item.BookId);
                if (book != null) total += book.Price * item.Quantity;
            }

            // Create the Order record
            var order = new Order
            {
                UserId      = userId.Value,
                OrderDate   = DateTime.Now,
                TotalAmount = total,
                Status      = "Pending"
            };

            _context.Orders.Add(order);
            _context.SaveChanges(); 

            
            foreach (var item in cart)
            {
                var book = books.FirstOrDefault(b => b.Id == item.BookId);
                if (book == null) continue;

                _context.OrderItems.Add(new OrderItem
                {
                    OrderId  = order.Id,
                    BookId   = item.BookId,
                    Quantity = item.Quantity,
                    Price    = book.Price
                });

                
                book.Stock = Math.Max(0, book.Stock - item.Quantity);
            }

            _context.SaveChanges();

            
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.SetInt32("CartCount", 0);

            
            return RedirectToAction("OrderSuccess", "Orders", new { orderId = order.Id });
        }
    }
}
