using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models.Entities
{
    [Table("Course")]
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public int Credits { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int MaxStudents { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        public int? FacultyID { get; set; }

        [Required]
        [StringLength(20)]
        public string Semester { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string AcademicYear { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Schedule { get; set; }

        [StringLength(50)]
        public string? Room { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        [ForeignKey("FacultyID")]
        public virtual Faculty? Faculty { get; set; }

        // QUAN TRỌNG: Navigation này phải có để AutoInclude hoạt động
        public virtual ICollection<ProgramCourse> ProgramCourses { get; set; } = new List<ProgramCourse>();

        public virtual ICollection<Enrollment>? Enrollments { get; set; }
    }
}