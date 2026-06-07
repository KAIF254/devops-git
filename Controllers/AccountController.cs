
using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Data;
using OnlineBookStore.Models;

namespace OnlineBookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ── Register (GET) ────────────────────────────────────
       
        public IActionResult Register()
        {
            return View();
        }

        // ── Register (POST) ───────────────────────────────────
        // Saves a new user with Role = "User"
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                
                bool emailExists = _context.Users.Any(u => u.Email == user.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(user);
                }

        
                user.Role = "User";

                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Account created! Please login.";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // ── Login (GET) ───────────────────────────────────────
        public IActionResult Login()
        {
            return View();
        }

        // ── Login (POST) ──────────────────────────────────────

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
            
                HttpContext.Session.SetInt32("UserId",    user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserEmail",user.Email);
                HttpContext.Session.SetString("UserRole", user.Role);  

                
                if (user.Role == "Admin")
                    return RedirectToAction("AdminDashboard", "Home");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password. Please try again.";
                return View();
            }
        }

        // ── Logout ────────────────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
