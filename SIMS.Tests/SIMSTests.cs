using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models.Entities;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace SIMS.Tests
{

    public class SIMSTests : IDisposable
    {
        private readonly SIMSContext _context;

        public SIMSTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<SIMSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SIMSContext(options);
            SeedTestData();
        }

        private void SeedTestData()
        {
            // Seed Roles
            var studentRole = new Role { RoleID = 1, RoleName = "Student" };
            var facultyRole = new Role { RoleID = 2, RoleName = "Faculty" };
            var adminRole = new Role { RoleID = 3, RoleName = "Admin" };
            _context.Roles.AddRange(studentRole, facultyRole, adminRole);

            // Seed Department
            var department = new Department
            {
                DepartmentID = 1,
                DepartmentName = "Computer Science",
                DepartmentCode = "CS"
            };
            _context.Departments.Add(department);

            // Seed Academic Program
            var program = new AcademicProgram
            {
                ProgramID = 1,
                ProgramName = "Software Engineering",
                ProgramCode = "SE",
                DepartmentID = 1
            };
            _context.AcademicPrograms.Add(program);

            // Seed Users
            var studentUser = new User
            {
                UserID = 1,
                Username = "student1",
                PasswordHash = ComputeSha256("password123"),
                Email = "student1@sims.edu",
                FullName = "John Doe",
                RoleID = 1
            };
            var facultyUser = new User
            {
                UserID = 2,
                Username = "faculty1",
                PasswordHash = ComputeSha256("password123"),
                Email = "faculty1@sims.edu",
                FullName = "Dr. Jane Smith",
                RoleID = 2
            };
            _context.Users.AddRange(studentUser, facultyUser);

            // Seed Faculty
            var faculty = new Faculty
            {
                FacultyID = 1,
                UserID = 2,
                EmployeeCode = "FAC001",
                DepartmentID = 1
            };
            _context.Faculties.Add(faculty);

            // Seed Student
            var student = new Student
            {
                StudentID = 1,
                UserID = 1,
                StudentCode = "STU001",
                ProgramID = 1,
                DepartmentID = 1,
                GPA = 3.5m,
                TotalCredits = 30,
                Status = "Active"
            };
            _context.Students.Add(student);

            // Seed Course
            var course = new Course
            {
                CourseID = 1,
                CourseCode = "CS101",
                CourseName = "Introduction to Programming",
                Credits = 3,
                MaxStudents = 30,
                DepartmentID = 1,
                FacultyID = 1,
                Semester = "Fall",
                AcademicYear = "2024-2025",
                IsActive = true
            };
            _context.Courses.Add(course);

            // Seed Enrollment
            var enrollment = new Enrollment
            {
                EnrollmentID = 1,
                StudentID = 1,
                CourseID = 1,
                EnrollmentDate = DateTime.Now,
                Status = "Enrolled"
            };
            _context.Enrollments.Add(enrollment);

            _context.SaveChanges();
        }

        private byte[] ComputeSha256(string input)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ==================== TEST CASES ====================

        /// <summary>
        /// Test Case 1: Verify that a new student can be created successfully
        /// </summary>
        [Fact]
        public void Test_CreateStudent_Success()
        {
            // Arrange
            var newUser = new User
            {
                UserID = 10,
                Username = "newstudent",
                PasswordHash = ComputeSha256("test123"),
                Email = "newstudent@sims.edu",
                FullName = "New Student",
                RoleID = 1
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();

            var newStudent = new Student
            {
                StudentID = 10,
                UserID = 10,
                StudentCode = "STU010",
                ProgramID = 1,
                Status = "Active"
            };

            // Act
            _context.Students.Add(newStudent);
            _context.SaveChanges();

            // Assert
            var savedStudent = _context.Students.Find(10);
            Assert.NotNull(savedStudent);
            Assert.Equal("STU010", savedStudent.StudentCode);
            Assert.Equal("Active", savedStudent.Status);
        }

        /// <summary>
        /// Test Case 2: Verify that a student can be retrieved by ID
        /// </summary>
        [Fact]
        public void Test_GetStudentById_ReturnsCorrectStudent()
        {
            // Act
            var student = _context.Students.Find(1);

            // Assert
            Assert.NotNull(student);
            Assert.Equal("STU001", student.StudentCode);
            Assert.Equal(3.5m, student.GPA);
        }

        /// <summary>
        /// Test Case 3: Verify that a course can be created with valid data
        /// </summary>
        [Fact]
        public void Test_CreateCourse_Success()
        {
            // Arrange
            var newCourse = new Course
            {
                CourseID = 10,
                CourseCode = "CS202",
                CourseName = "Data Structures",
                Credits = 4,
                MaxStudents = 25,
                DepartmentID = 1,
                Semester = "Spring",
                AcademicYear = "2024-2025",
                IsActive = true
            };

            // Act
            _context.Courses.Add(newCourse);
            _context.SaveChanges();

            // Assert
            var savedCourse = _context.Courses.Find(10);
            Assert.NotNull(savedCourse);
            Assert.Equal("CS202", savedCourse.CourseCode);
            Assert.Equal("Data Structures", savedCourse.CourseName);
            Assert.Equal(4, savedCourse.Credits);
        }

        /// <summary>
        /// Test Case 4: Verify that enrollment can be created for a student
        /// </summary>
        [Fact]
        public void Test_CreateEnrollment_Success()
        {
            // Arrange - create a new course first
            var newCourse = new Course
            {
                CourseID = 20,
                CourseCode = "CS303",
                CourseName = "Algorithms",
                Credits = 3,
                MaxStudents = 20,
                DepartmentID = 1,
                Semester = "Fall",
                AcademicYear = "2024-2025",
                IsActive = true
            };
            _context.Courses.Add(newCourse);
            _context.SaveChanges();

            var newEnrollment = new Enrollment
            {
                EnrollmentID = 10,
                StudentID = 1,
                CourseID = 20,
                EnrollmentDate = DateTime.Now,
                Status = "Enrolled"
            };

            // Act
            _context.Enrollments.Add(newEnrollment);
            _context.SaveChanges();

            // Assert
            var savedEnrollment = _context.Enrollments.Find(10);
            Assert.NotNull(savedEnrollment);
            Assert.Equal("Enrolled", savedEnrollment.Status);
        }

        /// <summary>
        /// Test Case 5: Verify that user authentication works with correct credentials
        /// </summary>
        [Fact]
        public void Test_UserAuthentication_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "student1";
            var password = "password123";
            var expectedHash = ComputeSha256(password);

            // Act
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(expectedHash, user.PasswordHash);
            Assert.Equal("student1", user.Username);
        }

        /// <summary>
        /// Test Case 6: Verify that user authentication fails with incorrect password
        /// </summary>
        [Fact]
        public void Test_UserAuthentication_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var username = "student1";
            var wrongPassword = "wrongpassword";
            var wrongHash = ComputeSha256(wrongPassword);

            // Act
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            // Assert
            Assert.NotNull(user);
            Assert.NotEqual(wrongHash, user.PasswordHash);
        }

        /// <summary>
        /// Test Case 7: Verify that student's GPA can be updated
        /// </summary>
        [Fact]
        public void Test_UpdateStudentGPA_Success()
        {
            // Arrange
            var student = _context.Students.Find(1);
            Assert.NotNull(student);
            var newGPA = 3.8m;

            // Act
            student.GPA = newGPA;
            _context.SaveChanges();

            // Assert
            var updatedStudent = _context.Students.Find(1);
            Assert.NotNull(updatedStudent);
            Assert.Equal(3.8m, updatedStudent.GPA);
        }

        /// <summary>
        /// Test Case 8: Verify that enrollment status can be updated to Completed
        /// </summary>
        [Fact]
        public void Test_UpdateEnrollmentStatus_ToCompleted()
        {
            // Arrange
            var enrollment = _context.Enrollments.Find(1);
            Assert.NotNull(enrollment);

            // Act
            enrollment.Status = "Completed";
            enrollment.FinalScore = 85.5m;
            _context.SaveChanges();

            // Assert
            var updatedEnrollment = _context.Enrollments.Find(1);
            Assert.NotNull(updatedEnrollment);
            Assert.Equal("Completed", updatedEnrollment.Status);
            Assert.Equal(85.5m, updatedEnrollment.FinalScore);
        }

        /// <summary>
        /// Test Case 9: Verify that a course can be deactivated
        /// </summary>
        [Fact]
        public void Test_DeactivateCourse_Success()
        {
            // Arrange
            var course = _context.Courses.Find(1);
            Assert.NotNull(course);
            Assert.True(course.IsActive);

            // Act
            course.IsActive = false;
            _context.SaveChanges();

            // Assert
            var updatedCourse = _context.Courses.Find(1);
            Assert.NotNull(updatedCourse);
            Assert.False(updatedCourse.IsActive);
        }

        /// <summary>
        /// Test Case 10: Verify that student can be deleted
        /// </summary>
        [Fact]
        public void Test_DeleteStudent_Success()
        {
            // Arrange - Create student to delete
            var userToDelete = new User
            {
                UserID = 100,
                Username = "deleteme",
                PasswordHash = ComputeSha256("delete123"),
                Email = "delete@sims.edu",
                FullName = "Delete Me",
                RoleID = 1
            };
            _context.Users.Add(userToDelete);
            _context.SaveChanges();

            var studentToDelete = new Student
            {
                StudentID = 100,
                UserID = 100,
                StudentCode = "DEL001",
                ProgramID = 1,
                Status = "Active"
            };
            _context.Students.Add(studentToDelete);
            _context.SaveChanges();

            // Verify student exists
            Assert.NotNull(_context.Students.Find(100));

            // Act
            _context.Students.Remove(studentToDelete);
            _context.SaveChanges();

            // Assert
            var deletedStudent = _context.Students.Find(100);
            Assert.Null(deletedStudent);
        }
    }
}
