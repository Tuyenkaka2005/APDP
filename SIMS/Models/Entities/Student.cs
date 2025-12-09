using System.ComponentModel.DataAnnotations;

namespace SIMS.Models.Entities
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        public string StudentCode { get; set; } = null!;
        public DateTime? AdmissionDate { get; set; }
        public string? AdmissionType { get; set; }
        public decimal GPA { get; set; } = 0;
        public int TotalCredits { get; set; } = 0;
        public string Status { get; set; } = "Active";

        public int ProgramID { get; set; }
        public AcademicProgram AcademicPrograms { get; set; } = null!;

        public int? DepartmentID { get; set; }
        public Department? Department { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
