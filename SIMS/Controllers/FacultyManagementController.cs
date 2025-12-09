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
    public class FacultyManagementController : Controller
    {
        private readonly SIMSContext _context;

        public FacultyManagementController(SIMSContext context)
        {
            _context = context;
        }

        // GET: FacultyManagement
        public async Task<IActionResult> Index(string searchString, int? departmentId, string qualification)
        {
            ViewBag.FullName = HttpContext.Session.GetString("FullName");

            var facultiesQuery = _context.Faculties
                .Include(f => f.User)
                .Include(f => f.Department)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(searchString))
            {
                facultiesQuery = facultiesQuery.Where(f =>
                    f.EmployeeCode.Contains(searchString) ||
                    f.User.FullName.Contains(searchString) ||
                    f.User.Email.Contains(searchString) ||
                    f.Specialization.Contains(searchString));
            }

            // Filter by Department
            if (departmentId.HasValue)
            {
                facultiesQuery = facultiesQuery.Where(f => f.DepartmentID == departmentId.Value);
            }

            // Filter by Qualification
            if (!string.IsNullOrEmpty(qualification))
            {
                facultiesQuery = facultiesQuery.Where(f => f.Qualification == qualification);
            }

            var faculties = await facultiesQuery.OrderBy(f => f.EmployeeCode).ToListAsync();

            // For filter dropdowns
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "DepartmentID", "DepartmentName");
            ViewBag.Qualifications = new SelectList(new[] { "Professor", "Master", "Bachelor", "Engineer" });
            ViewBag.SearchString = searchString;
            ViewBag.DepartmentId = departmentId;
            ViewBag.Qualification = qualification;

            return View(faculties);
        }

        // GET: FacultyManagement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .Include(f => f.User)
                .Include(f => f.Department)
                .FirstOrDefaultAsync(m => m.FacultyID == id);

            if (faculty == null)
            {
                return NotFound();
            }

            // Get courses taught by this faculty
            var courses = await _context.Courses
                .Where(c => c.FacultyID == faculty.FacultyID)
                .OrderByDescending(c => c.AcademicYear)
                .ThenBy(c => c.CourseCode)
                .ToListAsync();

            ViewBag.Courses = courses;

            return View(faculty);
        }

        // GET: FacultyManagement/Create
        public IActionResult Create()
        {
            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName");

            var model = new CreateFacultyViewModel
            {
                HireDate = DateTime.Now
            };

            return View(model);
        }

        // POST: FacultyManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFacultyViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists");
                    ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                    return View(model);
                }

                // Check if email exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                    return View(model);
                }

                // Check if employee code exists
                if (await _context.Faculties.AnyAsync(f => f.EmployeeCode == model.EmployeeCode))
                {
                    ModelState.AddModelError("EmployeeCode", "Employee code already exists");
                    ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                    return View(model);
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Get Faculty role
                    var facultyRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Faculty");
                    if (facultyRole == null)
                    {
                        ModelState.AddModelError("", "The role Faculty was not found in the system.");
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
                        RoleID = facultyRole.RoleID,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Create Faculty
                    var faculty = new Faculty
                    {
                        UserID = user.UserID,
                        EmployeeCode = model.EmployeeCode,
                        HireDate = model.HireDate,
                        Qualification = model.Qualification,
                        Specialization = model.Specialization,
                        Position = model.Position,
                        DepartmentID = model.DepartmentID,
                        OfficeLocation = model.OfficeLocation
                    };

                    _context.Faculties.Add(faculty);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    TempData["Success"] = "Faculty added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
            return View(model);
        }

        // GET: FacultyManagement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FacultyID == id);

            if (faculty == null)
            {
                return NotFound();
            }

            var model = new FacultyViewModel
            {
                FacultyID = faculty.FacultyID,
                UserID = faculty.UserID,
                EmployeeCode = faculty.EmployeeCode,
                HireDate = faculty.HireDate,
                Qualification = faculty.Qualification,
                Specialization = faculty.Specialization,
                Position = faculty.Position,
                DepartmentID = faculty.DepartmentID,
                OfficeLocation = faculty.OfficeLocation
            };

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
            ViewBag.UserFullName = faculty.User.FullName;

            return View(model);
        }

        // POST: FacultyManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FacultyViewModel model)
        {
            if (id != model.FacultyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var faculty = await _context.Faculties.FindAsync(id);
                    if (faculty == null)
                    {
                        return NotFound();
                    }

                    // Check if employee code changed and exists
                    if (faculty.EmployeeCode != model.EmployeeCode &&
                        await _context.Faculties.AnyAsync(f => f.EmployeeCode == model.EmployeeCode && f.FacultyID != id))
                    {
                        ModelState.AddModelError("EmployeeCode", "Employee code already exists");
                        ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
                        return View(model);
                    }

                    faculty.EmployeeCode = model.EmployeeCode;
                    faculty.HireDate = model.HireDate;
                    faculty.Qualification = model.Qualification;
                    faculty.Specialization = model.Specialization;
                    faculty.Position = model.Position;
                    faculty.DepartmentID = model.DepartmentID;
                    faculty.OfficeLocation = model.OfficeLocation;

                    _context.Update(faculty);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Faculty updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyExists(model.FacultyID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentID", "DepartmentName", model.DepartmentID);
            return View(model);
        }

        // GET: FacultyManagement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.Faculties
                .Include(f => f.User)
                .Include(f => f.Department)
                .FirstOrDefaultAsync(m => m.FacultyID == id);

            if (faculty == null)
            {
                return NotFound();
            }

            // Check if faculty has courses
            var courseCount = await _context.Courses.CountAsync(c => c.FacultyID == id);
            ViewBag.CourseCount = courseCount;

            return View(faculty);
        }

        // POST: FacultyManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faculty = await _context.Faculties
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FacultyID == id);

            if (faculty == null)
            {
                return NotFound();
            }

            // Check if faculty has courses
            var hasCourses = await _context.Courses.AnyAsync(c => c.FacultyID == id);
            if (hasCourses)
            {
                TempData["Error"] = "Cannot delete faculty who is teaching courses. Please remove the assignment first.";
                return RedirectToAction(nameof(Index));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete related records
                var grades = _context.Grades.Where(g => g.GradedByFacultyID == id);
                foreach (var grade in grades)
                {
                    grade.GradedByFacultyID = null;
                }

                // Delete faculty
                _context.Faculties.Remove(faculty);

                // Delete user
                if (faculty.User != null)
                {
                    _context.Users.Remove(faculty.User);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Faculty deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "An error occurred when deleting the faculty: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private bool FacultyExists(int id)
        {
            return _context.Faculties.Any(e => e.FacultyID == id);
        }
    }
}