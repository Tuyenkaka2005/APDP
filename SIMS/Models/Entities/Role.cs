namespace SIMS.Models.Entities
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = null!; // Admin, Faculty, Student
        public string? Description { get; set; }
        public string? Permissions { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
