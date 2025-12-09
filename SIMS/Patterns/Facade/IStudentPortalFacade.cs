using SIMS.Models.Entities;
using SIMS.Patterns.Adapter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.Patterns.Facade
{
    public interface IStudentPortalFacade
    {
        Task<Student> GetStudentProfileAsync(int userId);
        Task<object> GetDashboardDataAsync(int userId);
        Task<List<Enrollment>> GetEnrollmentsAsync(int studentId, string semester = null, string academicYear = null, string status = null);
        Task<List<ExternalGrade>> GetExternalGradesAsync(string studentCode);
        Task<Enrollment> GetCourseDetailAsync(int enrollmentId, int studentId);
        Task<(List<Grade> Internal, List<ExternalGrade> External)> GetAllGradesAsync(int studentId, string studentCode);
        Task<(List<string> Semesters, List<string> Years)> GetFilterOptionsAsync(int studentId);
    }
}
