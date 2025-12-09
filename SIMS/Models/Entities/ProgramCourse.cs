using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models.Entities
{
    [Table("ProgramCourse")]
    public class ProgramCourse
    {
        [Key]
        public int ProgramCourseID { get; set; }

        [Required]
        public int ProgramID { get; set; }

        [Required]
        public int CourseID { get; set; }

        // Khớp với DB
        public bool IsRequired { get; set; } = true;

        public int? SemesterRecommended { get; set; }

        [StringLength(500)]
        public string? PrerequisiteCourses { get; set; }

        // Navigation Properties
        [ForeignKey("ProgramID")]
        public virtual AcademicProgram? AcademicProgram { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course? Course { get; set; }
    }
}