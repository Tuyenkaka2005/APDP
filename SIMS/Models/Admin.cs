using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string EmployeeCode { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        public string Position { get; set; } = "Administrator";

        // Navigation
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; } = null!;
    }
}