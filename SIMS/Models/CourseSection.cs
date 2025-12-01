using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class CourseSection
    {
        [Key]
        public int SectionId { get; set; }

        public int CourseId { get; set; }
        public int FacultyId { get; set; }

        [Required]
        public string Semester { get; set; } = "HK1";
        public int AcademicYear { get; set; } = DateTime.Now.Year;

        public int Capacity { get; set; } = 60;
        public int EnrolledCount { get; set; } = 0;

        public string? Room { get; set; }
        public string? Schedule { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual Faculty Faculty { get; set; } = null!;
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}