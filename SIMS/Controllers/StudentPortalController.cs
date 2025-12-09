using Microsoft.AspNetCore.Mvc;
using SIMS.Filters;
using SIMS.Patterns.Facade;
using System.Reflection;

namespace SIMS.Controllers
{
    [AuthorizeRole("Student")]
    public class StudentPortalController : Controller
    {
        private readonly IStudentPortalFacade _studentFacade;

        public StudentPortalController(IStudentPortalFacade studentFacade)
        {
            _studentFacade = studentFacade;
        }

        // GET: StudentPortal/Index (Dashboard)
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "Auth");

            // Facade & Factory usage inside
            // The Facade returns an anonymous object (from Factory), we might need to cast or use dynamic in View
            // But usually, it's safer to have a concrete ViewModel. 
            // For now, we use dynamic/object as per Factory implementation.
            var dashboardData = await _studentFacade.GetDashboardDataAsync(userId.Value);
            
            if (dashboardData == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin sinh viên!";
                return RedirectToAction("Login", "Auth");
            }

            // Using reflection to get properties from the anonymous object created by Factory
            // Or better yet, modify Factory to return a specific ViewModel class. 
            // For improved demonstration, let's just pass the data object to View.
            // But wait, the original View expects ViewBag.Student etc.
            
            // Let's unpack the object manually for the View, or update the View.
            // Since we must maintain View compatibility or minimal changes:
            var type = dashboardData.GetType();
            ViewBag.Student = type.GetProperty("Student")?.GetValue(dashboardData);
            ViewBag.SchoolName = type.GetProperty("SchoolName")?.GetValue(dashboardData); // Singleton data
            ViewBag.SchoolAddress = type.GetProperty("SchoolAddress")?.GetValue(dashboardData);

            // Re-fetch statistics for dashboard view (Facade could have aggregated this too)
            // Ideally Facade should return a DashboardViewModel containing all stats.
            
            // For this exercise, I will trust the Facade logic. 
            // Note: The previous logic had stats. Let's add stats to Facade/Factory later or here?
            // To keep it clean, I should have put everything in DashboardViewModel. 
            
            // Let's populate the additional ViewBags that the original view expected, 
            // or we assume they are part of the new DashboardViewModel?
            // The original view used: ViewBag.TotalEnrollments, ViewBag.CompletedCourses, ViewBag.CurrentSemesterEnrollments.
            
            // I'll re-implement the stats logic using the Facade's helper methods if available, 
            // or relying on the student object if it has collections loaded.
            // The Facade.GetStudentProfileAsync includes Enrollments? No, it includes AcademicPrograms, Department.
            // We need to fetch specific lists.
            
            var student = (SIMS.Models.Entities.Student)ViewBag.Student;
            var enrollments = await _studentFacade.GetEnrollmentsAsync(student.StudentID);
            
            ViewBag.TotalEnrollments = enrollments.Count;
            ViewBag.CompletedCourses = enrollments.Count(e => e.Status == "Completed");
            ViewBag.CurrentSemesterEnrollments = enrollments
                .Where(e => e.Status == "Enrolled")
                .OrderBy(e => e.Course.CourseName)
                .Take(5)
                .ToList();

            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            return View();
        }

        // GET: StudentPortal/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var student = await _studentFacade.GetStudentProfileAsync(userId.Value);
            if (student == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin sinh viên!";
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            return View(student);
        }

        // GET: StudentPortal/MyCourses
        public async Task<IActionResult> MyCourses(string semester = null, string academicYear = null, string status = null)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var student = await _studentFacade.GetStudentProfileAsync(userId.Value);
            if (student == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin sinh viên!";
                return RedirectToAction("Login", "Auth");
            }

            var enrollments = await _studentFacade.GetEnrollmentsAsync(student.StudentID, semester, academicYear, status);
            var (semesters, years) = await _studentFacade.GetFilterOptionsAsync(student.StudentID);

            ViewBag.Semesters = semesters;
            ViewBag.AcademicYears = years;
            ViewBag.SelectedSemester = semester;
            ViewBag.SelectedAcademicYear = academicYear;
            ViewBag.SelectedStatus = status;
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            return View(enrollments);
        }

        // GET: StudentPortal/CourseDetail/5
        public async Task<IActionResult> CourseDetail(int? id)
        {
            if (id == null) return NotFound();

            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var student = await _studentFacade.GetStudentProfileAsync(userId.Value);
            if (student == null) return RedirectToAction("Login", "Auth");

            var enrollment = await _studentFacade.GetCourseDetailAsync(id.Value, student.StudentID);

            if (enrollment == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin đăng ký môn học!";
                return RedirectToAction(nameof(MyCourses));
            }

            // For Grade, the Facade isn't fully utilized for single item yet, but we can query it or add method.
            // Let's stick closer to the pattern: Use Facade or existing context? 
            // The requirement implies Facade. The Facade has GetAllGradesAsync.
            // Let's implement GetGradeForEnrollment in Facade or just filter from GetAllGradesAsync (less efficient).
            // Or better, add dedicated method to Facade later? 
            // For now, I'll assume we can't change Facade Interface too much on the fly without updating file.
            // Actually, I can update Facade if needed. But let's check GetAllGradesAsync.
            
            var (internalGrades, _) = await _studentFacade.GetAllGradesAsync(student.StudentID, student.StudentCode);
            var grade = internalGrades.FirstOrDefault(g => g.EnrollmentID == enrollment.EnrollmentID);

            ViewBag.Grade = grade;
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            return View(enrollment);
        }

        // GET: StudentPortal/Grades
        public async Task<IActionResult> Grades(string semester = null, string academicYear = null)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var student = await _studentFacade.GetStudentProfileAsync(userId.Value);
            if (student == null) return RedirectToAction("Login", "Auth");

            var (internalGrades, externalGrades) = await _studentFacade.GetAllGradesAsync(student.StudentID, student.StudentCode);

            // Filter logic for Internal Grades
            var filteredGrades = internalGrades.AsEnumerable(); 
            if (!string.IsNullOrEmpty(semester)) filteredGrades = filteredGrades.Where(g => g.Enrollment.Semester == semester);
            if (!string.IsNullOrEmpty(academicYear)) filteredGrades = filteredGrades.Where(g => g.Enrollment.AcademicYear == academicYear);

            // Prepare View Data
            // We pass Internal Grades to the View as Model (compatible with previous view)
            // But we also pass Adapter data (External Grades) via ViewBag
            
            var (semesters, years) = await _studentFacade.GetFilterOptionsAsync(student.StudentID);

            ViewBag.Student = student;
            ViewBag.Semesters = semesters;
            ViewBag.AcademicYears = years;
            ViewBag.SelectedSemester = semester;
            ViewBag.SelectedAcademicYear = academicYear;
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            // Expose Adapter Data
            ViewBag.ExternalGrades = externalGrades; 

            return View(filteredGrades.ToList());
        }
    }
}