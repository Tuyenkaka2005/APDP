using System.Data;

namespace SIMS.Models.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }

        public int RoleID { get; set; }
        public Role Role { get; set; } = null!;

        // Navigation
        public Student? Student { get; set; }
        public Faculty? Faculty { get; set; }
        public Admin? Admin { get; set; }
    }
}
