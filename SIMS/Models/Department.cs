using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        public string DepartmentCode { get; set; } = null!;

        [Required]
        public string DepartmentName { get; set; } = null!;

        public string? Location { get; set; }

        public virtual ICollection<AcademicProgram> Programs { get; set; } = new List<AcademicProgram>();
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
        public virtual ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
    }
}