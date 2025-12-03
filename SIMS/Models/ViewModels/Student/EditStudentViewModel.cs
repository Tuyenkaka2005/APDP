using System.ComponentModel.DataAnnotations;

namespace SIMS.Models.ViewModels.Student
{
    public class EditStudentViewModel
    {
        public int StudentId { get; set; }
        [Required] public string StudentCode { get; set; } = null!;
        [Required] public string FullName { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;
        public int AcademicProgramId { get; set; }

        [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal GPA { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}