using System.ComponentModel.DataAnnotations;
using SIMS.Models.Entities;

namespace SIMS.ViewModels
{
    public class AssignCourseViewModel
    {
        public int CourseId { get; set; }

        // KHÔNG Required vì được load từ DB
        public Course? Course { get; set; }

        [Required(ErrorMessage = "Please select a program")]
        [Display(Name = "Program")]
        public int ProgramId { get; set; }

        [Display(Name = "Required")]
        public bool IsRequired { get; set; } = true;

        [Display(Name = "Recommended Semester")]
        public int? SemesterRecommended { get; set; }

        // Danh sách môn tiên quyết
        public List<int> SelectedPrerequisiteCourseIds { get; set; } = new();
    }
}