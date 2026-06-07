using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Data;
using OnlineBookStore.Models;

namespace OnlineBookStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        private bool IsLoggedIn()
            => HttpContext.Session.GetInt32("UserId") != null;


        private bool IsAdmin()
            => HttpContext.Session.GetString("UserRole") == "Admin";

        // ── URL: /Orders/Checkout?bookId=5 (GET) ─────────────

        public IActionResult Checkout(int bookId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return NotFound();

            if (book.Stock <= 0)
            {
                TempData["ErrorMessage"] = "Sorry, this book is out of stock!";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var category = _context.Categories.FirstOrDefault(c => c.Id == book.CategoryId);
            ViewBag.CategoryName = category?.Name ?? "";

            return View(book);
        }

        // ── URL: /Orders/Checkout (POST) ──────────────────────
        
        [HttpPost]
        public IActionResult Checkout(int bookId, int quantity)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var book = _context.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return NotFound();

            
            if (quantity < 1) quantity = 1;
            if (quantity > book.Stock)
            {
                TempData["ErrorMessage"] = $"Only {book.Stock} copies available.";
                return RedirectToAction("Checkout", new { bookId });
            }

            int userId = HttpContext.Session.GetInt32("UserId")!.Value;
            decimal totalAmount = book.Price * quantity;

            
            var order = new Order
            {
                UserId      = userId,
                OrderDate   = DateTime.Now,
                TotalAmount = totalAmount,
                Status      = "Pending"
            };
            _context.Orders.Add(order);
            _context.SaveChanges(); 

            var item = new OrderItem
            {
                OrderId  = order.Id,
                BookId   = book.Id,
                Quantity = quantity,
                Price    = book.Price
            };
            _context.OrderItems.Add(item);

            book.Stock -= quantity;
            _context.Books.Update(book);

            _context.SaveChanges();

            return RedirectToAction("OrderSuccess", new { orderId = order.Id });
        }

        // ── URL: /Orders/OrderSuccess?orderId=5 ──────────────
        public IActionResult OrderSuccess(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return NotFound();

            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToList();

            var bookIds = orderItems.Select(oi => oi.BookId).ToList();
            var books   = _context.Books.Where(b => bookIds.Contains(b.Id)).ToList();

            ViewBag.Order      = order;
            ViewBag.OrderItems = orderItems;
            ViewBag.Books      = books;

            return View();
        }

        // ── URL: /Orders/MyOrders ─────────────────────────────
        public IActionResult MyOrders()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int userId = HttpContext.Session.GetInt32("UserId")!.Value;

            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var orderIds = orders.Select(o => o.Id).ToList();
            var allItems = _context.OrderItems
                .Where(oi => orderIds.Contains(oi.OrderId))
                .ToList();
            var bookIds  = allItems.Select(oi => oi.BookId).Distinct().ToList();
            var allBooks = _context.Books.Where(b => bookIds.Contains(b.Id)).ToList();

            ViewBag.AllItems = allItems;
            ViewBag.AllBooks = allBooks;

            return View(orders);
        }

        // ── URL: /Orders/UpdateStatus (POST) ──────────────────
        [HttpPost]
        public IActionResult UpdateStatus(int orderId, string status)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Orders.Update(order);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"Order #{orderId} status updated to {status}.";
            }

            return RedirectToAction("ManageOrders", "Admin");
        }
    }
}
