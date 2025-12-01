using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public class Faculty
    {
        [Key]
        public int FacultyId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string EmployeeCode { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        public int? DepartmentId { get; set; }

        public string Qualification { get; set; } = string.Empty;

        // Navigation
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } = null!;

        public virtual Department? Department { get; set; }
        public virtual ICollection<CourseSection> TeachingSections { get; set; } = new List<CourseSection>();
    }
}