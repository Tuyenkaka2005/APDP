using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        public int StudentId { get; set; }
        public int SectionId { get; set; }

        public DateTime EnrollDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Enrolled"; // Enrolled, Dropped, Completed

        public decimal? MidtermScore { get; set; }
        public decimal? FinalScore { get; set; }
        public decimal? TotalScore { get; set; }
        public string? LetterGrade { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual CourseSection Section { get; set; } = null!;
    }
}