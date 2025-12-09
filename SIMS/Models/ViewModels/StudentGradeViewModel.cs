using SIMS.Models.Entities;

namespace SIMS.Models.ViewModels
{
    public class StudentGradeViewModel
    {
        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public int CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;

        // Grade information
        public int? GradeID { get; set; }
        public decimal? FinalGrade { get; set; }
        public string? LetterGrade { get; set; }
        public string GradeStatus { get; set; } = "Pending";
        public string? Comments { get; set; }
        public DateTime? GradeDate { get; set; }
    }

    public class CourseGradesViewModel
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public List<StudentGradeViewModel> Students { get; set; } = new();
    }
}
