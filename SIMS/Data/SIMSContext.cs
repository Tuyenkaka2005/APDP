// Data/SIMSContext.cs
using Microsoft.EntityFrameworkCore;
using SIMS.Models.Entities;

namespace SIMS.Data
{
    public partial class SIMSContext : DbContext
    {
        public SIMSContext(DbContextOptions<SIMSContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<AcademicProgram> AcademicPrograms { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<ProgramCourse> ProgramCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === TÊN BẢNG KHỚP VỚI DB ===
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Faculty>().ToTable("Faculty");
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<Department>().ToTable("Department");

            // QUAN TRỌNG: Trong DB script là "AcademicProgram" KHÔNG có "s"
            modelBuilder.Entity<AcademicProgram>().ToTable("AcademicPrograms");

            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Grade>().ToTable("Grade");
            modelBuilder.Entity<ProgramCourse>().ToTable("ProgramCourse");

            // === Primary Keys ===
            modelBuilder.Entity<User>().HasKey(e => e.UserID);
            modelBuilder.Entity<Role>().HasKey(e => e.RoleID);
            modelBuilder.Entity<Student>().HasKey(e => e.StudentID);
            modelBuilder.Entity<Faculty>().HasKey(e => e.FacultyID);
            modelBuilder.Entity<Admin>().HasKey(e => e.AdminID);
            modelBuilder.Entity<Department>().HasKey(e => e.DepartmentID);
            modelBuilder.Entity<AcademicProgram>().HasKey(e => e.ProgramID);
            modelBuilder.Entity<Course>().HasKey(e => e.CourseID);
            modelBuilder.Entity<Enrollment>().HasKey(e => e.EnrollmentID);
            modelBuilder.Entity<Grade>().HasKey(e => e.GradeID);
            modelBuilder.Entity<ProgramCourse>().HasKey(e => e.ProgramCourseID);

            // === Unique Indexes ===
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Department>().HasIndex(d => d.DepartmentCode).IsUnique();
            modelBuilder.Entity<AcademicProgram>().HasIndex(p => p.ProgramCode).IsUnique();
            modelBuilder.Entity<Student>().HasIndex(s => s.StudentCode).IsUnique();
            modelBuilder.Entity<Faculty>().HasIndex(f => f.EmployeeCode).IsUnique();
            modelBuilder.Entity<Course>().HasIndex(c => c.CourseCode).IsUnique();
            modelBuilder.Entity<ProgramCourse>()
                .HasIndex(pc => new { pc.ProgramID, pc.CourseID })
                .IsUnique();

            // === Relationships ===
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserID);

            modelBuilder.Entity<Faculty>()
                .HasOne(f => f.User)
                .WithOne(u => u.Faculty)
                .HasForeignKey<Faculty>(f => f.UserID);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserID);

            modelBuilder.Entity<AcademicProgram>()
                .HasOne(p => p.Department)
                .WithMany(d => d.AcademicPrograms)
                .HasForeignKey(p => p.DepartmentID);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.AcademicPrograms)
                .WithMany(p => p.Students)
                .HasForeignKey(s => s.ProgramID);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentID)
                .IsRequired(false);

            modelBuilder.Entity<Faculty>()
                .HasOne(f => f.Department)
                .WithMany(d => d.Faculties)
                .HasForeignKey(f => f.DepartmentID)
                .IsRequired(false);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentID);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Faculty)
                .WithMany(f => f.Courses)
                .HasForeignKey(c => c.FacultyID)
                .IsRequired(false);
            modelBuilder.Entity<Course>()
                .Navigation(c => c.ProgramCourses)
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .AutoInclude();

            // === Course - Auto Include ===
            modelBuilder.Entity<Course>()
                .Navigation(c => c.ProgramCourses)
                .AutoInclude();

            modelBuilder.Entity<ProgramCourse>()
                .Navigation(pc => pc.AcademicProgram)
                .AutoInclude();

            // === ProgramCourse Configuration ===
            modelBuilder.Entity<ProgramCourse>(entity =>
            {
                entity.HasKey(pc => pc.ProgramCourseID);

                entity.HasIndex(pc => new { pc.ProgramID, pc.CourseID }).IsUnique();

                entity.HasOne(pc => pc.AcademicProgram)
                      .WithMany(p => p.ProgramCourses)
                      .HasForeignKey(pc => pc.ProgramID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pc => pc.Course)
                      .WithMany(c => c.ProgramCourses)
                      .HasForeignKey(pc => pc.CourseID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // === Enrollment ===
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentID);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseID);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentID, e.CourseID, e.Semester, e.AcademicYear })
                .IsUnique();

            // === Grade ===
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Enrollment)
                .WithOne(e => e.Grade)
                .HasForeignKey<Grade>(g => g.EnrollmentID);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.GradedByFaculty)
                .WithMany(f => f.GradedRecords)
                .HasForeignKey(g => g.GradedByFacultyID)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}