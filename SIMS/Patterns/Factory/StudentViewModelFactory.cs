using SIMS.Models.Entities;
using SIMS.Models.ViewModels;
using SIMS.Services;
using System.Threading.Tasks;

namespace SIMS.Patterns.Factory
{
    public class StudentViewModelFactory : IViewModelFactory
    {
        private readonly ISchoolInfoService _schoolInfoService;

        public StudentViewModelFactory(ISchoolInfoService schoolInfoService)
        {
            _schoolInfoService = schoolInfoService;
        }

        public async Task<object> CreateViewModelAsync(Student student, string type)
        {
            switch (type.ToLower())
            {
                case "dashboard":
                    return new
                    {
                        Student = student,
                        SchoolName = _schoolInfoService.GetSchoolName(),
                        SchoolAddress = _schoolInfoService.GetSchoolAddress()
                    };
                
                case "profile":
                    // Returning the Student entity directly as per existing view, 
                    // but wrapping it if we had a specific ProfileViewModel
                    return student; 

                default:
                    return null;
            }
        }
    }
}
