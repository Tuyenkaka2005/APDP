// Models/Entities/Grade.cs
namespace SIMS.Models.Entities
{
    public class Grade
    {
        public int GradeID { get; set; }

        public int EnrollmentID { get; set; }
        public Enrollment Enrollment { get; set; } = null!;

        public int StudentID { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseID { get; set; }
        public Course Course { get; set; } = null!;

        public decimal? FinalGrade { get; set; }
        public string? LetterGrade { get; set; }             // A, B+, C...
        public string GradeStatus { get; set; } = "Pending"; // Passed, Failed, Incomplete

        public DateTime GradeDate { get; set; } = DateTime.Now;

        public int? GradedByFacultyID { get; set; }
        public Faculty? GradedByFaculty { get; set; }

        public string? Comments { get; set; }
    }
}