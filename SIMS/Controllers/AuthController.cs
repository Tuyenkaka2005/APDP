// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using SIMS.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SIMS.Patterns.Singleton;

namespace SIMS.Controllers
{
    public class AuthController : Controller
    {
        private readonly SIMSContext _context;

        public AuthController(SIMSContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login(string returnUrl = null)
        {
            // Nếu đã đăng nhập rồi thì redirect về dashboard
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                var role = HttpContext.Session.GetString("RoleName");
                return RedirectToAction("Index", GetDashboardController(role));
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Please enter full account and password!";
                return View();
            }

            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                TempData["Error"] = "Account does not exist!";
                LoginLogger.Instance.LogLogin(username, false, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
                return View();
            }

            // So sánh mật khẩu (SHA256 - như dữ liệu mẫu)
            var inputHash = ComputeSha256(password);
            if (!user.PasswordHash.SequenceEqual(inputHash))
            {
                TempData["Error"] = "Password is incorrect!";
                LoginLogger.Instance.LogLogin(username, false, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
                return View();
            }

            // Đăng nhập thành công → Lưu vào Session
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("RoleName", user.Role.RoleName);
            HttpContext.Session.SetString("Username", user.Username);

            user.LastLogin = DateTime.Now;
            _context.SaveChanges();

            LoginLogger.Instance.LogLogin(username, true, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

            // Chuyển hướng theo returnUrl hoặc dashboard
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", GetDashboardController(user.Role.RoleName));
        }

        private string GetDashboardController(string roleName) => roleName switch
        {
            "Admin" => "Admin",
            "Faculty" => "Faculty",
            "Student" => "StudentPortal",
            _ => "Home"
        };

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Auth/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        private static byte[] ComputeSha256(string input)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }
}