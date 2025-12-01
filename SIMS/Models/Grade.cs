using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        public int EnrollmentId { get; set; }
        public decimal FinalGrade { get; set; }
        public string LetterGrade { get; set; } = null!;
        public DateTime GradeDate { get; set; } = DateTime.Now;
        public int? GradedByFacultyId { get; set; }

        public virtual Enrollment Enrollment { get; set; } = null!;
        public virtual Faculty? GradedByFaculty { get; set; }
    }
}