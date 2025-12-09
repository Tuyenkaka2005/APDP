using System.ComponentModel.DataAnnotations;

namespace SIMS.Models.ViewModels
{
    public class FacultyViewModel
    {
        public int FacultyID { get; set; }

        [Required(ErrorMessage = "Please select a user account")]
        [Display(Name = "User Account")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Please enter a faculty code")]
        [Display(Name = "Faculty Code")]
        [StringLength(20)]
        public string EmployeeCode { get; set; }

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [Display(Name = "Qualification")]
        [StringLength(100)]
        public string? Qualification { get; set; }

        [Display(Name = "Specialization")]
        [StringLength(255)]
        public string? Specialization { get; set; }

        [Display(Name = "Position")]
        [StringLength(100)]
        public string? Position { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }

        [Display(Name = "Office Location")]
        [StringLength(255)]
        public string? OfficeLocation { get; set; }

        // For display
        public string? UserFullName { get; set; }
        public string? DepartmentName { get; set; }
    }

    public class CreateFacultyViewModel
    {
        // User Info
        [Required(ErrorMessage = "Please enter a username")]
        [Display(Name = "Username")]
        [StringLength(100)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [Display(Name = "Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter an email address")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255)]
        public string Email { get; set; }

        [Display(Name = "Phone number")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please enter a full name")]
        [Display(Name = "Full name")]
        [StringLength(255)]
        public string FullName { get; set; }

        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Address")]
        [StringLength(500)]
        public string? Address { get; set; }

        // Faculty Info
        [Required(ErrorMessage = "Please enter an employee code")]
        [Display(Name = "Employee code")]
        [StringLength(20)]
        public string EmployeeCode { get; set; }

        [Display(Name = "Hire date")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [Display(Name = "Qualification")]
        [StringLength(100)]
        public string? Qualification { get; set; }

        [Display(Name = "Specialization")]
        [StringLength(255)]
        public string? Specialization { get; set; }

        [Display(Name = "Position")]
        [StringLength(100)]
        public string? Position { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }

        [Display(Name = "Office location")]
        [StringLength(255)]
        public string? OfficeLocation { get; set; }
    }
}