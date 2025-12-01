using Microsoft.AspNetCore.Identity;

namespace SIMS.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Quan hệ 1-1
        public Student? Student { get; set; }
        public Faculty? Faculty { get; set; }
        public Admin? Admin { get; set; }
    }
}