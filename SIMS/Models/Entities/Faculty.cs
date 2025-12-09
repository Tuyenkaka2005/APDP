// Models/Entities/Faculty.cs
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SIMS.Models.Entities
{
    public class Faculty
    {
        public int FacultyID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public string EmployeeCode { get; set; } = null!;

        public DateTime? HireDate { get; set; }

        public string? Qualification { get; set; }   // Tiến sĩ, Thạc sĩ...
        public string? Specialization { get; set; }
        public string? Position { get; set; }        // Lecturer, Professor...

        public int? DepartmentID { get; set; }
        public Department? Department { get; set; }

        public string? OfficeLocation { get; set; }

        // Navigation
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Grade> GradedRecords { get; set; } = new List<Grade>();
    }
}