// Models/Entities/Enrollment.cs
using System.Diagnostics;

namespace SIMS.Models.Entities
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }

        public int StudentID { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseID { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Enrolled";     // Enrolled, Completed, Dropped

        public int AttendanceCount { get; set; } = 0;
        public decimal? MidtermScore { get; set; }
        public decimal? FinalScore { get; set; }

        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }

        // Navigation
        public Grade? Grade { get; set; }
    }
}