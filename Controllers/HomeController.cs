using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Data;

namespace OnlineBookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── URL: /  or  /Home/Index ──────────────────────────
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserRole") == "Admin")
                return RedirectToAction("AdminDashboard");

            var featuredBooks = _context.Books
                .OrderByDescending(b => b.Id)
                .Take(8)
                .ToList();

            var topRankedBooks = _context.Books
                .OrderByDescending(b => b.Price)
                .Take(6)
                .ToList();

            var categories = _context.Categories.ToList();

            ViewBag.Categories    = categories;
            ViewBag.TopRanked     = topRankedBooks;

            return View(featuredBooks);
        }

        // ── URL: /Home/AdminDashboard ─────────────────────────
        public IActionResult AdminDashboard()
        {
            string? role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                TempData["ErrorMessage"] = "Access denied. Admin only area.";
                return RedirectToAction("Index");
            }

            ViewBag.TotalBooks   = _context.Books.Count();
            ViewBag.TotalUsers   = _context.Users.Count();
            ViewBag.TotalOrders  = _context.Orders.Count();
            ViewBag.TotalRevenue = _context.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;

            var recentOrders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToList();

            var users = _context.Users.ToList();

            var allBooks = _context.Books.ToList();

            var categories = _context.Categories.ToList();

            ViewBag.RecentOrders = recentOrders;
            ViewBag.Users        = users;
            ViewBag.AllBooks     = allBooks;
            ViewBag.Categories   = categories;

            return View();
        }

        // ── URL: /Home/Error ──────────────────────────────────
        public IActionResult Error()
        {
            return View();
        }
    }
}
