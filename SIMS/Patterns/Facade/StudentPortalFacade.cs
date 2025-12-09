using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models.Entities;
using SIMS.Patterns.Adapter;
using SIMS.Patterns.Factory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Patterns.Facade
{
    public class StudentPortalFacade : IStudentPortalFacade
    {
        private readonly SIMSContext _context;
        private readonly IExternalGradeSystem _externalGradeSystem;
        private readonly IViewModelFactory _viewModelFactory;

        public StudentPortalFacade(SIMSContext context, IExternalGradeSystem externalGradeSystem, IViewModelFactory viewModelFactory)
        {
            _context = context;
            _externalGradeSystem = externalGradeSystem;
            _viewModelFactory = viewModelFactory;
        }

        public async Task<Student> GetStudentProfileAsync(int userId)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.AcademicPrograms)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.UserID == userId);
        }

        public async Task<object> GetDashboardDataAsync(int userId)
        {
            var student = await GetStudentProfileAsync(userId);
            if (student == null) return null;

            return await _viewModelFactory.CreateViewModelAsync(student, "dashboard");
        }

        public async Task<List<Enrollment>> GetEnrollmentsAsync(int studentId, string semester = null, string academicYear = null, string status = null)
        {
            var query = _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Faculty)
                        .ThenInclude(f => f.User)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Department)
                .Where(e => e.StudentID == studentId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(semester)) query = query.Where(e => e.Semester == semester);
            if (!string.IsNullOrEmpty(academicYear)) query = query.Where(e => e.AcademicYear == academicYear);
            if (!string.IsNullOrEmpty(status)) query = query.Where(e => e.Status == status);

            return await query
                .OrderByDescending(e => e.AcademicYear)
                .ThenByDescending(e => e.Semester)
                .ThenBy(e => e.Course.CourseName)
                .ToListAsync();
        }

        public async Task<List<ExternalGrade>> GetExternalGradesAsync(string studentCode)
        {
            // Adapter Pattern usage
            return await Task.FromResult(_externalGradeSystem.GetGradesForStudent(studentCode));
        }
        
        public async Task<Enrollment> GetCourseDetailAsync(int enrollmentId, int studentId)
        {
             return await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Faculty)
                        .ThenInclude(f => f.User)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Department)
                .FirstOrDefaultAsync(e => e.EnrollmentID == enrollmentId && e.StudentID == studentId);
        }

        public async Task<(List<Grade> Internal, List<ExternalGrade> External)> GetAllGradesAsync(int studentId, string studentCode)
        {
            var internalGrades = await _context.Grades
                .Include(g => g.Enrollment)
                    .ThenInclude(e => e.Course)
                .Include(g => g.GradedByFaculty)
                    .ThenInclude(f => f.User)
                .Where(g => g.StudentID == studentId)
                .ToListAsync();

            var externalGrades = await GetExternalGradesAsync(studentCode);

            return (internalGrades, externalGrades);
        }

        public async Task<(List<string> Semesters, List<string> Years)> GetFilterOptionsAsync(int studentId)
        {
            var semesters = await _context.Enrollments
                .Where(e => e.StudentID == studentId)
                .Select(e => e.Semester)
                .Distinct()
                .ToListAsync();

            var years = await _context.Enrollments
                .Where(e => e.StudentID == studentId)
                .Select(e => e.AcademicYear)
                .Distinct()
                .ToListAsync();

            return (semesters, years);
        }
    }
}
