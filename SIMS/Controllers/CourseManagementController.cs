using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Filters;
using SIMS.Models.Entities;
using SIMS.Models.ViewModels;
using SIMS.ViewModels;

namespace SIMS.Controllers
{
    [AuthorizeRole("Admin")]
    public class CourseManagementController : Controller
    {
        private readonly SIMSContext _context;

        public CourseManagementController(SIMSContext context)
        {
            _context = context;
        }

        // GET: CourseManagement/Index
        public async Task<IActionResult> Index(
            string searchString,
            int? departmentId,
            int? facultyId,
            string semester,
            string academicYear,
            bool? isActive)
        {
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var coursesQuery = _context.Courses
                .Include(c => c.Faculty!.User)
                .Include(c => c.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();
                coursesQuery = coursesQuery.Where(c =>
                    c.CourseCode.Contains(searchString) || c.CourseName.Contains(searchString));
            }

            if (departmentId.HasValue) coursesQuery = coursesQuery.Where(c => c.DepartmentID == departmentId.Value);
            if (facultyId.HasValue) coursesQuery = coursesQuery.Where(c => c.FacultyID == facultyId.Value);
            if (!string.IsNullOrEmpty(semester)) coursesQuery = coursesQuery.Where(c => c.Semester == semester);
            if (!string.IsNullOrEmpty(academicYear)) coursesQuery = coursesQuery.Where(c => c.AcademicYear == academicYear);
            if (isActive.HasValue) coursesQuery = coursesQuery.Where(c => c.IsActive == isActive.Value);

            var courses = await coursesQuery
                .OrderBy(c => c.CourseCode)
                .Select(c => new CourseViewModel
                {
                    CourseID = c.CourseID,
                    CourseCode = c.CourseCode,
                    CourseName = c.CourseName,
                    Credits = c.Credits,
                    Description = c.Description,
                    MaxStudents = c.MaxStudents,
                    DepartmentID = c.DepartmentID,
                    FacultyID = c.FacultyID,
                    Semester = c.Semester,
                    AcademicYear = c.AcademicYear,
                    Schedule = c.Schedule,
                    Room = c.Room,
                    IsActive = c.IsActive,
                    DepartmentName = c.Department!.DepartmentName,
                    FacultyName = c.Faculty != null ? c.Faculty.User!.FullName : "Not assigned yet"
                })
                .ToListAsync();

            // Dropdowns
            ViewBag.Departments = new SelectList(await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync(), "DepartmentID", "DepartmentName", departmentId);
            ViewBag.Faculties = new SelectList(
                await _context.Faculties
                    .Include(f => f.User)
                    .Select(f => new { f.FacultyID, FullName = f.User!.FullName })
                    .OrderBy(x => x.FullName)
                    .ToListAsync(),
                "FacultyID", "FullName", facultyId);

            ViewBag.Semesters = new SelectList(new[] { "1", "2", "3" }, semester);
            ViewBag.AcademicYears = new SelectList(new[] { "2024-2025", "2025-2026", "2026-2027" }, academicYear);

            ViewBag.SearchString = searchString;
            ViewBag.DepartmentId = departmentId;
            ViewBag.FacultyId = facultyId;
            ViewBag.Semester = semester;
            ViewBag.AcademicYear = academicYear;
            ViewBag.IsActive = isActive;

            return View(courses);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new CourseViewModel { IsActive = true, MaxStudents = 100 });
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Courses.AnyAsync(c => c.CourseCode == model.CourseCode))
                {
                    ModelState.AddModelError("CourseCode", "Course code already exists!");
                }
                else
                {
                    var course = new Course
                    {
                        CourseCode = model.CourseCode,
                        CourseName = model.CourseName,
                        Credits = model.Credits,
                        Description = model.Description,
                        MaxStudents = model.MaxStudents,
                        DepartmentID = model.DepartmentID,
                        FacultyID = model.FacultyID,
                        Semester = model.Semester,
                        AcademicYear = model.AcademicYear,
                        Schedule = model.Schedule,
                        Room = model.Room,
                        IsActive = model.IsActive
                    };

                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Course added successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }

            await LoadDropdowns(model.DepartmentID, model.FacultyID);
            return View(model);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var model = new CourseViewModel
            {
                CourseID = course.CourseID,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Credits = course.Credits,
                Description = course.Description,
                MaxStudents = course.MaxStudents,
                DepartmentID = course.DepartmentID,
                FacultyID = course.FacultyID,
                Semester = course.Semester,
                AcademicYear = course.AcademicYear,
                Schedule = course.Schedule,
                Room = course.Room,
                IsActive = course.IsActive
            };

            await LoadDropdowns(model.DepartmentID, model.FacultyID);
            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseViewModel model)
        {
            if (id != model.CourseID) return NotFound();

            if (ModelState.IsValid)
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null) return NotFound();

                if (await _context.Courses.AnyAsync(c => c.CourseCode == model.CourseCode && c.CourseID != id))
                {
                    ModelState.AddModelError("CourseCode", "Course code already exists!");
                }
                else
                {
                    course.CourseCode = model.CourseCode;
                    course.CourseName = model.CourseName;
                    course.Credits = model.Credits;
                    course.Description = model.Description;
                    course.MaxStudents = model.MaxStudents;
                    course.DepartmentID = model.DepartmentID;
                    course.FacultyID = model.FacultyID;
                    course.Semester = model.Semester;
                    course.AcademicYear = model.AcademicYear;
                    course.Schedule = model.Schedule;
                    course.Room = model.Room;
                    course.IsActive = model.IsActive;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Course updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }

            await LoadDropdowns(model.DepartmentID, model.FacultyID);
            return View(model);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Faculty!.User)
                .Include(c => c.Enrollments!)
                .FirstOrDefaultAsync(c => c.CourseID == id);

            if (course == null) return NotFound();

            var model = new CourseViewModel
            {
                CourseID = course.CourseID,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Credits = course.Credits,
                Description = course.Description,
                MaxStudents = course.MaxStudents,
                DepartmentID = course.DepartmentID,
                FacultyID = course.FacultyID,
                Semester = course.Semester,
                AcademicYear = course.AcademicYear,
                Schedule = course.Schedule,
                Room = course.Room,
                IsActive = course.IsActive,
                DepartmentName = course.Department?.DepartmentName,
                FacultyName = course.Faculty?.User?.FullName ?? "Not assigned yet"
            };

            ViewBag.Enrollments = course.Enrollments ?? new List<Enrollment>();
            ViewBag.EnrollmentCount = course.Enrollments?.Count ?? 0;
            return View(model);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Faculty!.User)
                .FirstOrDefaultAsync(c => c.CourseID == id);

            if (course == null) return NotFound();

            var model = new CourseViewModel
            {
                CourseID = course.CourseID,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Credits = course.Credits,
                DepartmentName = course.Department?.DepartmentName,
                FacultyName = course.Faculty?.User?.FullName ?? "Not assigned yet"
            };

            ViewBag.EnrollmentCount = await _context.Enrollments.CountAsync(e => e.CourseID == id);
            return View(model);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int CourseID)
        {
            var hasEnrollments = await _context.Enrollments.AnyAsync(e => e.CourseID == CourseID);
            if (hasEnrollments)
            {
                TempData["Error"] = "Cannot delete course with enrollments!";
                return RedirectToAction(nameof(Index));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.ProgramCourses.RemoveRange(_context.ProgramCourses.Where(pc => pc.CourseID == CourseID));
                _context.Courses.Remove(new Course { CourseID = CourseID });
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["Success"] = "Course deleted successfully!";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Error deleting course.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ==================== ASSIGN TO PROGRAM ====================

        // GET: AssignToProgram
        [HttpGet]
        public async Task<IActionResult> AssignToProgram(int id)
        {
            Console.WriteLine($"=== GET AssignToProgram: CourseID={id} ===");

            try
            {
                var course = await _context.Courses
                    .Include(c => c.Department)
                    .Include(c => c.ProgramCourses)
                        .ThenInclude(pc => pc.AcademicProgram)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(c => c.CourseID == id);

                if (course == null)
                {
                    Console.WriteLine("Course NOT FOUND!");
                    TempData["Error"] = "Course NOT FOUND!";
                    return RedirectToAction(nameof(Index));
                }

                course.ProgramCourses ??= new List<ProgramCourse>();

                Console.WriteLine($"Course: {course.CourseName}");
                Console.WriteLine($"ProgramCourses Count: {course.ProgramCourses.Count}");

                var model = new AssignCourseViewModel
                {
                    CourseId = course.CourseID,
                    Course = course,
                    IsRequired = true
                };

                await LoadAssignViewBags(course.CourseID);

                var programs = ViewBag.Programs as SelectList;
                Console.WriteLine($"Available Programs: {programs?.Count() ?? 0}");

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GET: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: AssignToProgram
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToProgram(AssignCourseViewModel model)
        {
            Console.WriteLine("=== POST AssignToProgram START ===");
            Console.WriteLine($"CourseId: {model.CourseId}");
            Console.WriteLine($"ProgramId: {model.ProgramId}");
            Console.WriteLine($"IsRequired: {model.IsRequired}");
            Console.WriteLine($"SemesterRecommended: {model.SemesterRecommended}");

            if (model.SelectedPrerequisiteCourseIds != null)
            {
                Console.WriteLine($"Prerequisites Count: {model.SelectedPrerequisiteCourseIds.Count}");
                Console.WriteLine($"Prerequisites: {string.Join(", ", model.SelectedPrerequisiteCourseIds)}");
            }
            else
            {
                Console.WriteLine("Prerequisites: NULL");
            }

            try
            {
                // Check ModelState
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("=== ModelState INVALID ===");
                    foreach (var key in ModelState.Keys)
                    {
                        var state = ModelState[key];
                        if (state.Errors.Count > 0)
                        {
                            Console.WriteLine($"Key: {key}");
                            foreach (var error in state.Errors)
                            {
                                Console.WriteLine($"  Error: {error.ErrorMessage}");
                                if (error.Exception != null)
                                {
                                    Console.WriteLine($"  Exception: {error.Exception.Message}");
                                }
                            }
                        }
                    }

                    model.Course = await _context.Courses
                        .Include(c => c.Department)
                        .Include(c => c.ProgramCourses)
                            .ThenInclude(pc => pc.AcademicProgram)
                        .FirstOrDefaultAsync(c => c.CourseID == model.CourseId);

                    await LoadAssignViewBags(model.CourseId);

                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Invalid model state: {errors}";
                    return View(model);
                }

                Console.WriteLine("=== ModelState VALID - Checking duplicate ===");

                var exists = await _context.ProgramCourses
                    .AnyAsync(pc => pc.ProgramID == model.ProgramId && pc.CourseID == model.CourseId);

                Console.WriteLine($"Already exists: {exists}");

                if (exists)
                {
                    TempData["Error"] = "Course already assigned to this program!";
                    return RedirectToAction(nameof(AssignToProgram), new { id = model.CourseId });
                }

                string? prereq = null;
                if (model.SelectedPrerequisiteCourseIds?.Any() == true)
                {
                    prereq = string.Join(",", model.SelectedPrerequisiteCourseIds);
                    Console.WriteLine($"Saving prerequisites: {prereq}");
                }

                Console.WriteLine("=== Creating ProgramCourse ===");

                var programCourse = new ProgramCourse
                {
                    ProgramID = model.ProgramId,
                    CourseID = model.CourseId,
                    IsRequired = model.IsRequired,
                    SemesterRecommended = model.SemesterRecommended,
                    PrerequisiteCourses = prereq
                };

                Console.WriteLine($"ProgramCourse: ProgramID={programCourse.ProgramID}, CourseID={programCourse.CourseID}");

                _context.ProgramCourses.Add(programCourse);

                Console.WriteLine("=== Calling SaveChangesAsync ===");
                var rowsAffected = await _context.SaveChangesAsync();

                Console.WriteLine($"=== SUCCESS: {rowsAffected} rows affected ===");
                Console.WriteLine($"New ProgramCourseID: {programCourse.ProgramCourseID}");

                TempData["Success"] = "Course assigned to program successfully!";
                return RedirectToAction(nameof(AssignToProgram), new { id = model.CourseId });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine("=== DATABASE ERROR ===");
                Console.WriteLine($"Message: {dbEx.Message}");
                Console.WriteLine($"InnerException: {dbEx.InnerException?.Message}");

                TempData["Error"] = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}";

                model.Course = await _context.Courses
                    .Include(c => c.Department)
                    .Include(c => c.ProgramCourses).ThenInclude(pc => pc.AcademicProgram)
                    .FirstOrDefaultAsync(c => c.CourseID == model.CourseId);

                await LoadAssignViewBags(model.CourseId);
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== GENERAL ERROR ===");
                Console.WriteLine($"Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");

                TempData["Error"] = $"General error: {ex.Message}";

                model.Course = await _context.Courses
                    .Include(c => c.Department)
                    .Include(c => c.ProgramCourses).ThenInclude(pc => pc.AcademicProgram)
                    .FirstOrDefaultAsync(c => c.CourseID == model.CourseId);

                await LoadAssignViewBags(model.CourseId);
                return View(model);
            }
        }

        // POST: RemoveFromProgram
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromProgram(int programCourseId, int courseId)
        {
            var pc = await _context.ProgramCourses.FindAsync(programCourseId);
            if (pc != null)
            {
                _context.ProgramCourses.Remove(pc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Course removed from program successfully!";
            }
            return RedirectToAction(nameof(AssignToProgram), new { id = courseId });
        }

        // ==================== TEST ACTIONS ====================

        [HttpGet]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                Console.WriteLine("=== Testing Database ===");

                var programs = await _context.AcademicPrograms.ToListAsync();
                var courses = await _context.Courses.ToListAsync();
                var programCourses = await _context.ProgramCourses
                    .Include(pc => pc.AcademicProgram)
                    .Include(pc => pc.Course)
                    .ToListAsync();

                return Json(new
                {
                    Success = true,
                    ProgramCount = programs.Count,
                    CourseCount = courses.Count,
                    ProgramCourseCount = programCourses.Count,
                    Programs = programs.Select(p => new { p.ProgramID, p.ProgramName, p.IsActive }),
                    Courses = courses.Select(c => new { c.CourseID, c.CourseName }),
                    ProgramCourses = programCourses.Select(pc => new
                    {
                        pc.ProgramCourseID,
                        pc.ProgramID,
                        pc.CourseID,
                        pc.IsRequired,
                        pc.SemesterRecommended,
                        ProgramName = pc.AcademicProgram?.ProgramName,
                        CourseName = pc.Course?.CourseName
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Error = ex.Message });
            }
        }

        // ==================== HELPERS ====================

        private async Task LoadDropdowns(int? deptId = null, int? facultyId = null)
        {
            ViewBag.Departments = new SelectList(
                await _context.Departments.OrderBy(d => d.DepartmentName).ToListAsync(),
                "DepartmentID", "DepartmentName", deptId);

            ViewBag.Faculties = new SelectList(
                await _context.Faculties
                    .Include(f => f.User)
                    .Select(f => new { f.FacultyID, FullName = f.User!.FullName })
                    .OrderBy(x => x.FullName)
                    .ToListAsync(),
                "FacultyID", "FullName", facultyId);
        }

        private async Task LoadAssignViewBags(int courseId, int? selectedProgramId = null)
        {
            ViewBag.Programs = new SelectList(
                await _context.AcademicPrograms
                    .Where(p => p.IsActive == true || p.IsActive == null)
                    .OrderBy(p => p.ProgramName)
                    .ToListAsync(),
                "ProgramID", "ProgramName", selectedProgramId);

            ViewBag.AllCourses = await _context.Courses
                .Where(c => c.CourseID != courseId && c.IsActive == true)
                .OrderBy(c => c.CourseCode)
                .Select(c => new
                {
                    c.CourseID,
                    DisplayText = $"{c.CourseCode} - {c.CourseName}"
                })
                .ToListAsync();
        }
    }
}