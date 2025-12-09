using System.ComponentModel.DataAnnotations;

namespace SIMS.Models.ViewModels
{
    public class StudentViewModel
    {
        public int StudentID { get; set; }

        [Required(ErrorMessage = "Please select a user account")]
        [Display(Name = "User account")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Please enter a student code")]
        [Display(Name = "Student code")]
        [StringLength(20)]
        public string StudentCode { get; set; }

        [Display(Name = "Admission date")]
        [DataType(DataType.Date)]
        public DateTime? AdmissionDate { get; set; }

        [Display(Name = "Admission type")]
        [StringLength(50)]
        public string? AdmissionType { get; set; }

        [Display(Name = "GPA")]
        [Range(0, 4, ErrorMessage = "GPA must be between 0 and 4")]
        public decimal GPA { get; set; }

        [Display(Name = "Total credits")]
        [Range(0, 200, ErrorMessage = "Total credits must be between 0 and 200")]
        public int TotalCredits { get; set; }

        [Display(Name = "Status")]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        [Required(ErrorMessage = "Please select a program")]
        [Display(Name = "Program")]
        public int ProgramID { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }



        // For display
        public string? UserFullName { get; set; }
        public string? ProgramName { get; set; }
        public string? DepartmentName { get; set; }
    }

    public class CreateStudentViewModel
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

        // Student Info
        [Required(ErrorMessage = "Please enter a student code")]
        [Display(Name = "Student code")]
        [StringLength(20)]
        public string StudentCode { get; set; }

        [Display(Name = "Admission date")]
        [DataType(DataType.Date)]
        public DateTime? AdmissionDate { get; set; }

        [Display(Name = "Admission type")]
        public string? AdmissionType { get; set; }

        [Required(ErrorMessage = "Please select a program")]
        [Display(Name = "Program")]
        public int ProgramID { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }


    }
}