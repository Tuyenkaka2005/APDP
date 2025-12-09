using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models.Entities
{
    [Table("AcademicPrograms")] // Có chữ "s"
    public class AcademicProgram
    {
        [Key]
        public int ProgramID { get; set; }

        [Required]
        [StringLength(20)]
        public string ProgramCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ProgramName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? DegreeType { get; set; }

        public int? DurationYears { get; set; }

        public int? RequiredCredits { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        public virtual ICollection<ProgramCourse> ProgramCourses { get; set; } = new List<ProgramCourse>();

        public virtual ICollection<Student>? Students { get; set; }
    }
}