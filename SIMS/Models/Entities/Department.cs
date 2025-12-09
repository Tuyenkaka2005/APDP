// Models/Entities/Department.cs
using System.ComponentModel.DataAnnotations;

namespace SIMS.Models.Entities
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [Required]
        public string DepartmentCode { get; set; } = null!;   // CNTT, QTKD...

        [Required]
        public string DepartmentName { get; set; } = null!;

        public string? Description { get; set; }
        public int? HeadFacultyID { get; set; }
        public string? Location { get; set; }
        public string? HeadOfDepartment { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<AcademicProgram> AcademicPrograms { get; set; } = new List<AcademicProgram>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
    }
}