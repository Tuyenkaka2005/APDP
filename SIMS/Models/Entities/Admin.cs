// Models/Entities/Admin.cs
using System.ComponentModel.DataAnnotations;

namespace SIMS.Models.Entities
{
    public class Admin
    {
        public int AdminID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public string EmployeeCode { get; set; } = null!;

        public DateTime? HireDate { get; set; }
        public string? Position { get; set; }
        public string? DepartmentAssigned { get; set; }
    }
}