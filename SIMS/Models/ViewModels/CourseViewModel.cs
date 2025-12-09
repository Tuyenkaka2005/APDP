// ViewModels/CourseViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SIMS.Models.ViewModels
{
    public class CourseViewModel
    {
        public int? CourseID { get; set; }

        [Required(ErrorMessage = "Please enter course code")]
        [StringLength(20)]
        [Display(Name = "Course Code")]
        public string CourseCode { get; set; } = null!;

        [Required(ErrorMessage = "Please enter course name")]
        [StringLength(255)]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter number of credits")]
        [Range(1, 10, ErrorMessage = "Number of credits from 1 to 10")]
        [Display(Name = "Number of Credits")]
        public int Credits { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Maximum Students")]
        [Range(1, 500)]
        public int MaxStudents { get; set; } = 100;

        [Required(ErrorMessage = "Please select department")]
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        [Display(Name = "Faculty")]
        public int? FacultyID { get; set; }

        [Display(Name = "Semester")]
        [StringLength(20)]
        public string? Semester { get; set; }

        [Display(Name = "Academic Year")]
        [StringLength(20)]
        public string? AcademicYear { get; set; }

        [Display(Name = "Schedule")]
        [StringLength(500)]
        public string? Schedule { get; set; }

        [Display(Name = "Room")]
        [StringLength(50)]
        public string? Room { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        // ============== DÙNG CHO HIỂN THỊ (Details, Index) ==============
        public string? DepartmentName { get; set; }
        public string? FacultyName { get; set; }
    }
}