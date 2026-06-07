using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Data;
using OnlineBookStore.Models;

namespace OnlineBookStore.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper: check admin role from session
        private bool IsAdmin()
            => HttpContext.Session.GetString("UserRole") == "Admin";

        // ── URL: /Categories ──────────────────────────────────
        // PUBLIC — shows all categories as cards
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            // Pass book counts per category
            var bookCounts = _context.Books
                .GroupBy(b => b.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.BookCounts = bookCounts.ToDictionary(x => x.CategoryId, x => x.Count);
            return View(categories);
        }

        // ── URL: /Categories/Books/3 ──────────────────────────
        // PUBLIC — shows all books in a selected category
        public IActionResult Books(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();

            var books = _context.Books
                .Where(b => b.CategoryId == id)
                .ToList();

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId   = id;
            return View(books);
        }

        // ── URL: /Categories/Create (GET) ─────────────────────
        // ADMIN ONLY — shows empty category form
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // ── URL: /Categories/Create (POST) ────────────────────
        // ADMIN ONLY — saves new category
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"Category \"{category.Name}\" added!";
                return RedirectToAction("AdminDashboard", "Home");
            }
            return View(category);
        }

        // ── URL: /Categories/Edit/3 (GET) ─────────────────────
        // ADMIN ONLY
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        // ── URL: /Categories/Edit/3 (POST) ────────────────────
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Category updated!";
                return RedirectToAction("AdminDashboard", "Home");
            }
            return View(category);
        }

        // ── URL: /Categories/Delete (POST) ────────────────────
        // ADMIN ONLY — deletes a category
        [HttpPost]
        public IActionResult Delete(int id)
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
                TempData["SuccessMessage"] = "Category deleted!";
            }
            return RedirectToAction("AdminDashboard", "Home");
        }
    }
}
