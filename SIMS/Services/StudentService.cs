using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SIMS.Data;
using SIMS.Models;

namespace SIMS.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<StudentService> _logger;

        public StudentService(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<StudentService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<List<Student>> GetAllAsync(string? search = null, int? programId = null)
        {
            var query = _context.Students
                .Include(s => s.User)
                .Include(s => s.AcademicProgram)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    s.StudentCode.ToLower().Contains(search) ||
                    s.FullName.ToLower().Contains(search) ||
                    (s.User != null && s.User.Email != null && s.User.Email.ToLower().Contains(search)));
            }

            if (programId.HasValue)
            {
                // ĐÃ SỬA: ProgramId → AcademicProgramId
                query = query.Where(s => s.AcademicProgramId == programId);
            }

            var students = await query
                .OrderBy(s => s.StudentCode)
                .ToListAsync();
            
            _logger.LogInformation("GetAllAsync found {Count} students.", students.Count);

            return students;
        }

        public async Task<Student?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.AcademicProgram)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task CreateAsync(Student student, string password) // Modified signature
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(student.User, password); // Use student.User
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(student.User, "Student"); // Use student.User

                student.UserId = student.User.Id; // Use student.User.Id
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Student student)
        {
            var existing = await _context.Students
                .Include(s => s.User)
                .FirstAsync(s => s.StudentId == student.StudentId);

            existing.FullName = student.FullName;
            // ĐÃ SỬA: ProgramId → AcademicProgramId
            existing.AcademicProgramId = student.AcademicProgramId;
            existing.GPA = student.GPA;

            if (existing.User != null)
            {
                existing.User.FullName = student.FullName;
                existing.User.Email = student.User?.Email ?? existing.User.Email;
                existing.User.UserName = student.User?.UserName ?? existing.User.UserName;
                existing.User.PhoneNumber = student.User?.PhoneNumber ?? existing.User.PhoneNumber;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                _logger.LogWarning("Student with ID: {Id} not found for deletion.", id);
                return;
            }

            var user = student.User;

            _context.Enrollments.RemoveRange(student.Enrollments);
            _context.Students.Remove(student);
            
            await _context.SaveChangesAsync();

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<bool> StudentCodeExistsAsync(string code, int? excludeId = null)
        {
            return await _context.Students
                .AnyAsync(s => s.StudentCode == code && (excludeId == null || s.StudentId != excludeId));
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }
    }
}