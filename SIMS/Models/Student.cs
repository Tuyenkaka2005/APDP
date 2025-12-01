using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string StudentCode { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        public DateTime AdmissionDate { get; set; } = DateTime.Now;

        public int? ProgramId { get; set; }

        public decimal GPA { get; set; } = 0m;

        // Navigation
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } = null!;
    }
}