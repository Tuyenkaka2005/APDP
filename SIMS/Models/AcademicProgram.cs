using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class AcademicProgram
    {
        [Key]
        public int AcademicProgramId { get; set; }
       

        [Required]
        public string ProgramCode { get; set; } = null!;

        [Required]
        public string ProgramName { get; set; } = null!;

        public string DegreeType { get; set; } = "Bachelor"; // Bachelor, Master...
        public int DurationYears { get; set; } = 4;
        public int RequiredCredits { get; set; } = 140;

        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    }
}