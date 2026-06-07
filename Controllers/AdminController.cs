
//   - ManageUsers()       → List all registered users
//   - DeleteUser(id)      → Remove a user from the database
//   - ManageOrders()      → List all orders with status update
//   - ManageBooks()       → Admin books management table
//   - ManageCategories()  → Admin categories management table
//   - DeleteCategory(id)  → Delete a category
// ============================================================

using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Data;
using OnlineBookStore.Models;

namespace OnlineBookStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        private bool IsAdmin()
            => HttpContext.Session.GetString("UserRole") == "Admin";

        // ── URL: /Admin/ManageBooks ───────────────────────────
       
        public IActionResult ManageBooks()
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var books = _context.Books.ToList();
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            return View(books);
        }

        // ── URL: /Admin/ManageCategories ──────────────────────
        
        public IActionResult ManageCategories()
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var categories = _context.Categories.ToList();

            var bookCounts = _context.Books
                .GroupBy(b => b.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.BookCounts = bookCounts.ToDictionary(x => x.CategoryId, x => x.Count);
            return View(categories);
        }

        // ── URL: /Admin/DeleteCategory (POST) ─────────────────

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            return RedirectToAction("ManageCategories");
        }

        // ── URL: /Admin/ManageUsers ───────────────────────────
    
        public IActionResult ManageUsers()
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var users = _context.Users.ToList();
            return View(users);
        }

        // ── URL: /Admin/DeleteUser (POST) ─────────────────────
        
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction("ManageUsers");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User deleted successfully.";
            }

            return RedirectToAction("ManageUsers");
        }

        // ── URL: /Admin/ManageOrders ──────────────────────────
        
        public IActionResult ManageOrders()
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var orders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var users    = _context.Users.ToList();
            var orderIds = orders.Select(o => o.Id).ToList();
            var allItems = _context.OrderItems.Where(oi => orderIds.Contains(oi.OrderId)).ToList();
            var bookIds  = allItems.Select(oi => oi.BookId).Distinct().ToList();
            var allBooks = _context.Books.Where(b => bookIds.Contains(b.Id)).ToList();

            ViewBag.Users    = users;
            ViewBag.AllItems = allItems;
            ViewBag.AllBooks = allBooks;

            return View(orders);
        }
    }
}
