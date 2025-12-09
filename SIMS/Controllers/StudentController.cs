using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Filters;
using SIMS.Helpers;
using SIMS.Models.Entities;
using SIMS.Models.ViewModels;

namespace SIMS.Controllers
{
    [AuthorizeRole("Admin")]
    public class StudentController : Controller
    {
        private readonly SIMSContext _context;

        public StudentController(SIMSContext context)
        {
            _context = context;
        }

        // GET: Student
        public async Task<IActionResult> Index(string searchString, int? programId, int? departmentId)
        {
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var studentsQuery = _context.Students
                .Include(s => s.User)
                .Include(s => s.AcademicPrograms)
                .Include(s => s.Department)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchString))
            {
                studentsQuery = studentsQuery.Where(s =>
                    s.StudentCode.Contains(searchString) ||
                    s.User.FullName.Contains(searchString) ||
                    s.User.Email.Contains(searchString));
            }

            // Filter by Program
            if (programId.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.ProgramID == programId.Value);
            }

            // Filter by Department
            if (departmentId.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.DepartmentID == departmentId.Value);
            }

            var students = await studentsQuery.OrderBy(s => s.StudentCode).ToListAsync();

            // For filter dropdowns
            ViewBag.Programs = new SelectList(await _context.AcademicPrograms.ToListAsync(), "ProgramID", "ProgramName");
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentID", "DepartmentName");
            ViewBag.SearchString = searchString;
            ViewBag.ProgramId = programId;
            ViewBag.DepartmentId = departmentId;

            return View(students);
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.AcademicPrograms)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }

            // Get enrollments
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentID == student.StudentID)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            ViewBag.Enrollments = enrollments;

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName");
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");

            var model = new CreateStudentViewModel
            {
                AdmissionDate = DateTime.Now
            };

            return View(model);
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "The login name already exists.");
                    ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName", model.ProgramID);
                    ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                    return View(model);
                }

                // Check if email exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName", model.ProgramID);
                    ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                    return View(model);
                }

                // Check if student code exists
                if (await _context.Students.AnyAsync(s => s.StudentCode == model.StudentCode))
                {
                    ModelState.AddModelError("StudentCode", "Student code already exists.");
                    ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName", model.ProgramID);
                    ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                    return View(model);
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Get Student role
                    var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Student");
                    if (studentRole == null)
                    {
                        ModelState.AddModelError("", "Student role not found in the system.");
                        ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName", model.ProgramID);
                        ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                        return View(model);
                    }

                    // Create User
                    var user = new User
                    {
                        Username = model.Username,
                        PasswordHash = PasswordHelper.HashPassword(model.Password),
                        Email = model.Email,
                        Phone = model.Phone,
                        FullName = model.FullName,
                        DateOfBirth = model.DateOfBirth,
                        Address = model.Address,
                        RoleID = studentRole.RoleID,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Create Student
                    var student = new Student
                    {
                        UserID = user.UserID,
                        StudentCode = model.StudentCode,
                        AdmissionDate = model.AdmissionDate,
                        AdmissionType = model.AdmissionType,
                        ProgramID = model.ProgramID,
                        DepartmentID = model.DepartmentID,
                        GPA = 0,
                        TotalCredits = 0,
                        Status = "Active"
                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["Success"] = "Student added successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName", model.ProgramID);
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
            return View(model);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentViewModel
            {
                StudentID = student.StudentID,
                UserID = student.UserID,
                StudentCode = student.StudentCode,
                AdmissionDate = student.AdmissionDate,
                AdmissionType = student.AdmissionType,
                GPA = student.GPA,
                TotalCredits = student.TotalCredits,
                Status = student.Status,
                ProgramID = student.ProgramID,
                DepartmentID = student.DepartmentID
            };

            ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName", model.ProgramID);
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
            ViewBag.UserFullName = student.User.FullName;

            return View(model);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentViewModel model)
        {
            if (id != model.StudentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _context.Students.FindAsync(id);
                    if (student == null)
                    {
                        return NotFound();
                    }

                    // Check if student code changed and exists
                    if (student.StudentCode != model.StudentCode &&
                        await _context.Students.AnyAsync(s => s.StudentCode == model.StudentCode && s.StudentID != id))
                    {
                        ModelState.AddModelError("StudentCode", "Student code already exists.");
                        ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName", model.ProgramID);
                        ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                        return View(model);
                    }

                    student.StudentCode = model.StudentCode;
                    student.AdmissionDate = model.AdmissionDate;
                    student.AdmissionType = model.AdmissionType;
                    student.GPA = model.GPA;
                    student.TotalCredits = model.TotalCredits;
                    student.Status = model.Status;
                    student.ProgramID = model.ProgramID;
                    student.DepartmentID = model.DepartmentID;

                    _context.Update(student);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Student information updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(model.StudentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Programs = new SelectList(_context.AcademicPrograms, "ProgramID", "ProgramName", model.ProgramID);
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
            return View(model);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.AcademicPrograms)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete related records first
                var enrollments = _context.Enrollments.Where(e => e.StudentID == id);
                _context.Enrollments.RemoveRange(enrollments);

                var grades = _context.Grades.Where(g => g.StudentID == id);
                _context.Grades.RemoveRange(grades);

                // Delete student
                _context.Students.Remove(student);

                // Delete user
                if (student.User != null)
                {
                    _context.Users.Remove(student.User);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Student deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "An error occurred when deleting the student: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Student/AssignProgramCourses/5
        public async Task<IActionResult> AssignProgramCourses(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.AcademicPrograms)
                .FirstOrDefaultAsync(m => m.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }

            // Get courses in the program
            var programCourses = await _context.ProgramCourses
                .Include(pc => pc.Course)
                .Where(pc => pc.ProgramID == student.ProgramID)
                .ToListAsync();

            // Get existing enrollments
            var existingEnrollments = await _context.Enrollments
                .Where(e => e.StudentID == student.StudentID)
                .Select(e => e.CourseID)
                .ToListAsync();

            // Filter courses not yet enrolled
            var coursesToAssign = programCourses
                .Where(pc => !existingEnrollments.Contains(pc.CourseID))
                .Select(pc => pc.Course)
                .ToList();

            ViewBag.CoursesToAssign = coursesToAssign;

            return View(student);
        }

        // POST: Student/AssignProgramCourses/5
        [HttpPost, ActionName("AssignProgramCourses")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignProgramCoursesConfirmed(int id)
        {
            var student = await _context.Students
                .Include(s => s.AcademicPrograms)
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (student == null)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get courses in the program
                var programCourses = await _context.ProgramCourses
                    .Where(pc => pc.ProgramID == student.ProgramID)
                    .ToListAsync();

                // Get existing enrollments
                var existingEnrollments = await _context.Enrollments
                    .Where(e => e.StudentID == student.StudentID)
                    .Select(e => e.CourseID)
                    .ToListAsync();

                // Identify new courses
                var newCourses = programCourses
                    .Where(pc => !existingEnrollments.Contains(pc.CourseID))
                    .ToList();

                if (newCourses.Any())
                {
                    foreach (var pc in newCourses)
                    {
                        var enrollment = new Enrollment
                        {
                            StudentID = student.StudentID,
                            CourseID = pc.CourseID,
                            EnrollmentDate = DateTime.Now,
                            Status = "Enrolled",
                            Semester = "HK1", // Default for now, could be dynamic
                            AcademicYear = "2024-2025" // Default
                        };
                        _context.Enrollments.Add(enrollment);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Success"] = $"Assigned successfully {newCourses.Count} courses to the student!";
                }
                else
                {
                    TempData["Info"] = "The student has already enrolled in all courses in the program.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "An error occurred: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }
    }
}