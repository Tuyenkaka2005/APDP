using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        public string CourseCode { get; set; } = null!;

        [Required]
        public string CourseName { get; set; } = null!;

        public int Credits { get; set; } = 3;
        public string? Description { get; set; }

        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}