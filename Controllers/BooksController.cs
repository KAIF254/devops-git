// ============================================================
// FILE: Controllers/BooksController.cs
// PURPOSE: Manages everything related to Books.
//
//   PUBLIC (anyone):
//   - Index    → List/search books
//   - Details  → Show one book + suggested books
//
//   ADMIN ONLY (guarded):
//   - Create   → Add a new book
//   - Edit     → Update a book
//   - Delete   → Remove a book
//
// ADMIN GUARD: Every write action checks session UserRole == "Admin"
// ============================================================

using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Data;
using OnlineBookStore.Models;

namespace OnlineBookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // ── URL: /Books ───────────────────────────────────────
        // Public: Search + category filter + book grid
        public IActionResult Index(string searchTitle, int? categoryId)
        {
            var books = _context.Books.AsQueryable();

            
            if (!string.IsNullOrEmpty(searchTitle))
                books = books.Where(b => b.Title.Contains(searchTitle) ||
                                        b.Author.Contains(searchTitle));

            
            if (categoryId.HasValue && categoryId.Value > 0)
                books = books.Where(b => b.CategoryId == categoryId.Value);

            
            ViewBag.SearchTitle = searchTitle;
            ViewBag.CategoryId  = categoryId;
            ViewBag.Categories  = _context.Categories.ToList();

            return View(books.ToList());
        }

        // ── URL: /Books/Details/5 ─────────────────────────────
        // Public: Show full book info + suggested books (same category)
        public IActionResult Details(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            
            var category = _context.Categories.FirstOrDefault(c => c.Id == book.CategoryId);
            ViewBag.CategoryName = category?.Name ?? "Unknown";

            // Suggested books: same category, different book, up to 4
            var suggestedBooks = _context.Books
                .Where(b => b.CategoryId == book.CategoryId && b.Id != book.Id)
                .Take(4)
                .ToList();
            ViewBag.SuggestedBooks = suggestedBooks;

            return View(book);
        }

        // ── URL: /Books/Create (GET) ──────────────────────────
        // ADMIN ONLY — shows empty book form
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // ── URL: /Books/Create (POST) ─────────────────────────
        
        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"Book \"{book.Title}\" added successfully!";
                return RedirectToAction("AdminDashboard", "Home");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // ── URL: /Books/Edit/5 (GET) ──────────────────────────
        
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // ── URL: /Books/Edit/5 (POST) ────────────────────────
        // ADMIN ONLY — saves updated book
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"Book \"{book.Title}\" updated successfully!";
                return RedirectToAction("AdminDashboard", "Home");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(book);
        }

        // ── URL: /Books/Delete/5 (GET) ────────────────────────
        
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();
            return View(book);
        }

        // ── URL: /Books/Delete (POST) ─────────────────────────
        
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                TempData["SuccessMessage"] = $"Book deleted successfully.";
            }

            return RedirectToAction("AdminDashboard", "Home");
        }
    }
}
