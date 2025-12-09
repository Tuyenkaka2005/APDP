using Microsoft.AspNetCore.Mvc;
using SIMS.Filters;
using SIMS.Data;
using Microsoft.EntityFrameworkCore;

namespace SIMS.Controllers
{
    [AuthorizeRole("Admin")]
    public class AdminController : Controller
    {
        private readonly SIMSContext _context;

        public AdminController(SIMSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            // Đếm số lượng
            ViewBag.TotalStudents = await _context.Students.CountAsync();
            ViewBag.TotalFaculty = await _context.Faculties.CountAsync();
            ViewBag.TotalCourses = await _context.Courses.CountAsync();
            ViewBag.TotalDepartments = await _context.Departments.CountAsync();

            return View();
        }
    }
}