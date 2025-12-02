using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIMS.Models;
using SIMS.Models.ViewModels.Student;
using SIMS.Services;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentAdminController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IAcademicProgramService _programService;
        private readonly UserManager<AppUser> _userManager;

        public StudentAdminController(IStudentService studentService, IAcademicProgramService programService, UserManager<AppUser> userManager)
        {
            _studentService = studentService;
            _programService = programService;
            _userManager = userManager;
        }

        // GET: /StudentAdmin
        public async Task<IActionResult> Index(string? search, int? programId)
        {
            var students = await _studentService.GetAllAsync(search, programId);
            ViewBag.Search = search;
            ViewBag.ProgramId = programId;
            ViewBag.Programs = await _programService.GetAllAsync();
            return View(students);
        }

        // GET: /StudentAdmin/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Programs = await _programService.GetAllAsync();
            return View(new CreateStudentViewModel());
        }

        // POST: /StudentAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStudentViewModel model)
        {
            if (await _studentService.StudentCodeExistsAsync(model.StudentCode))
                ModelState.AddModelError("StudentCode", "Mã sinh viên đã tồn tại!");

            if (await _userManager.FindByEmailAsync(model.Email) != null)
                ModelState.AddModelError("Email", "Email đã được sử dụng!");

            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };

                var student = new Student
                {
                    StudentCode = model.StudentCode,
                    FullName = model.FullName,
                    AdmissionDate = DateTime.Today,
                    AcademicProgramId = model.AcademicProgramId, // ĐÃ SỬA: AcademicProgramId (không phải ProgramId)
                    GPA = model.GPA,
                    User = user
                };

                await _studentService.CreateAsync(student, model.Password);
                TempData["Success"] = "Thêm sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Programs = await _programService.GetAllAsync();
            return View(model);
        }

        // GET: /StudentAdmin/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetByIdWithDetailsAsync(id);
            if (student == null) return NotFound();

            var model = new EditStudentViewModel
            {
                StudentId = student.StudentId,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Email = student.User?.Email ?? "",
                AcademicProgramId = student.AcademicProgramId,
                GPA = student.GPA,
                PhoneNumber = student.User?.PhoneNumber
            };

            ViewBag.Programs = await _programService.GetAllAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Programs = await _programService.GetAllAsync();
                return View(model);
            }

            var student = await _studentService.GetByIdWithDetailsAsync(model.StudentId);
            if (student == null) return NotFound();

            student.StudentCode = model.StudentCode;
            student.FullName = model.FullName;
            student.AcademicProgramId = model.AcademicProgramId;
            student.GPA = model.GPA;

            if (student.User != null)
            {
                student.User.Email = model.Email;
                student.User.UserName = student.StudentCode; // Set UserName to StudentCode for uniqueness
                student.User.FullName = model.FullName;
                student.User.PhoneNumber = model.PhoneNumber;
            }

            await _studentService.UpdateAsync(student);
            TempData["Success"] = "Cập nhật sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /StudentAdmin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetByIdWithDetailsAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: /StudentAdmin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentService.DeleteAsync(id);
            TempData["Success"] = "Xóa sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}