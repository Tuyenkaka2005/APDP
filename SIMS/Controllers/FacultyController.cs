using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Filters;
using SIMS.Models.Entities;

namespace SIMS.Controllers
{
    [AuthorizeRole("Faculty")]
    public class FacultyController : Controller
    {
        private readonly SIMSContext _context;

        public FacultyController(SIMSContext context)
        {
            _context = context;
        }

        // GET: Faculty/Index - Dashboard
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            // Lấy thông tin giảng viên
            var faculty = await _context.Faculties
                .Include(f => f.User)
                .Include(f => f.Department)
                .FirstOrDefaultAsync(f => f.UserID == userId.Value);

            if (faculty == null)
            {
                TempData["Error"] = "No instructor information foundn!";
                return RedirectToAction("Login", "Auth");
            }

            // Lấy danh sách môn học đang dạy
            var courses = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Enrollments)
                .Where(c => c.FacultyID == faculty.FacultyID && c.IsActive)
                .OrderBy(c => c.CourseCode)
                .ToListAsync();

            // Thống kê
            ViewBag.Faculty = faculty;
            ViewBag.TotalCourses = courses.Count;
            ViewBag.TotalStudents = courses.Sum(c => c.Enrollments?.Count ?? 0);
            ViewBag.CurrentSemester = DateTime.Now.Month <= 6 ? "2" : "1";
            ViewBag.AcademicYear = $"{DateTime.Now.Year}-{DateTime.Now.Year + 1}";

            return View(courses);
        }

        // GET: Faculty/MyCourses - Danh sách lớp học
        public async Task<IActionResult> MyCourses(string semester, string academicYear)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(f => f.UserID == userId.Value);

            if (faculty == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var coursesQuery = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Enrollments!)
                    .ThenInclude(e => e.Student)
                        .ThenInclude(s => s!.User)
                .Where(c => c.FacultyID == faculty.FacultyID && c.IsActive);

            if (!string.IsNullOrEmpty(semester))
            {
                coursesQuery = coursesQuery.Where(c => c.Semester == semester);
            }

            if (!string.IsNullOrEmpty(academicYear))
            {
                coursesQuery = coursesQuery.Where(c => c.AcademicYear == academicYear);
            }

            var courses = await coursesQuery
                .OrderBy(c => c.CourseCode)
                .ToListAsync();

            // Dropdowns
            ViewBag.Semesters = new[] { "1", "2", "3" };
            ViewBag.AcademicYears = new[] { "2024-2025", "2025-2026", "2026-2027" };
            ViewBag.SelectedSemester = semester;
            ViewBag.SelectedAcademicYear = academicYear;

            return View(courses);
        }

        // GET: Faculty/CourseDetails/5 - Chi tiết lớp học
        public async Task<IActionResult> CourseDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(f => f.UserID == userId.Value);

            if (faculty == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Kiểm tra giảng viên có quyền xem lớp này không
            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Faculty!.User)
                .Include(c => c.Enrollments!)
                    .ThenInclude(e => e.Student)
                        .ThenInclude(s => s!.User)
                .Include(c => c.Enrollments!)
                    .ThenInclude(e => e.Student)
                        .ThenInclude(s => s!.AcademicPrograms)
                .FirstOrDefaultAsync(c => c.CourseID == id && c.FacultyID == faculty.FacultyID);

            if (course == null)
            {
                TempData["Error"] = "Course not found or you do not have access!";
                return RedirectToAction(nameof(MyCourses));
            }

            // Sắp xếp sinh viên theo MSSV
            if (course.Enrollments != null)
            {
                course.Enrollments = course.Enrollments
                    .OrderBy(e => e.Student?.StudentCode)
                    .ToList();
            }

            ViewBag.EnrollmentCount = course.Enrollments?.Count ?? 0;
            ViewBag.MaxStudents = course.MaxStudents;

            return View(course);
        }

        // GET: Faculty/StudentDetails/5 - Chi tiết sinh viên
        public async Task<IActionResult> StudentDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(f => f.UserID == userId.Value);

            if (faculty == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Lấy thông tin sinh viên
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.AcademicPrograms)
                .Include(s => s.Enrollments!)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (student == null)
            {
                TempData["Error"] = "Student not found!";
                return RedirectToAction(nameof(MyCourses));
            }

            // Kiểm tra giảng viên có dạy sinh viên này không
            var isTeachingStudent = await _context.Enrollments
                .AnyAsync(e => e.StudentID == id &&
                          e.Course!.FacultyID == faculty.FacultyID);

            if (!isTeachingStudent)
            {
                TempData["Error"] = "You do not have permission to view this student's information!";
                return RedirectToAction(nameof(MyCourses));
            }

            // Lấy các môn học giảng viên đang dạy mà sinh viên đăng ký
            var enrollments = student.Enrollments?
                .Where(e => e.Course!.FacultyID == faculty.FacultyID)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToList();

            ViewBag.MyEnrollments = enrollments;

            return View(student);
        }

        // GET: Faculty/StudentList - Danh sách tất cả sinh viên đang học
        public async Task<IActionResult> StudentList(string searchString, int? courseId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(f => f.UserID == userId.Value);

            if (faculty == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Lấy danh sách sinh viên
            var studentsQuery = _context.Enrollments
                .Include(e => e.Student)
                    .ThenInclude(s => s!.User)
                .Include(e => e.Student)
                    .ThenInclude(s => s!.AcademicPrograms)
                .Include(e => e.Course)
                .Where(e => e.Course!.FacultyID == faculty.FacultyID)
                .Select(e => e.Student)
                .Distinct();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                studentsQuery = studentsQuery.Where(s =>
                    s!.StudentCode.Contains(searchString) ||
                    s.User!.FullName.Contains(searchString) ||
                    s.User.Email.Contains(searchString));
            }

            if (courseId.HasValue)
            {
                var studentIds = await _context.Enrollments
                    .Where(e => e.CourseID == courseId.Value)
                    .Select(e => e.StudentID)
                    .ToListAsync();

                studentsQuery = studentsQuery.Where(s => studentIds.Contains(s!.StudentID));
            }

            var students = await studentsQuery
                .OrderBy(s => s!.StudentCode)
                .ToListAsync();

            // Lấy danh sách môn học để filter
            var courses = await _context.Courses
                .Where(c => c.FacultyID == faculty.FacultyID && c.IsActive)
                .OrderBy(c => c.CourseCode)
                .ToListAsync();

            ViewBag.Courses = courses;
            ViewBag.SearchString = searchString;
            ViewBag.SelectedCourseId = courseId;

            return View(students);
        }

        // GET: Faculty/Profile - Thông tin cá nhân
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var faculty = await _context.Faculties
                .Include(f => f.User)
                .Include(f => f.Department)
                .Include(f => f.Courses)
                .FirstOrDefaultAsync(f => f.UserID == userId.Value);

            if (faculty == null)
            {
                TempData["Error"] = "Faculty information not found!";
                return RedirectToAction("Login", "Auth");
            }

            // Thống kê
            ViewBag.TotalCourses = faculty.Courses?.Count(c => c.IsActive) ?? 0;
            ViewBag.TotalStudents = await _context.Enrollments
                .Where(e => e.Course!.FacultyID == faculty.FacultyID)
                .Select(e => e.StudentID)
                .Distinct()
                .CountAsync();

            return View(faculty);
        }
    }
}