using System.Collections.Generic;

namespace SIMS.Patterns.Adapter
{
    // Target Interface
    public interface IExternalGradeSystem
    {
        List<ExternalGrade> GetGradesForStudent(string studentCode);
    }

    public class ExternalGrade
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public double Score { get; set; }
        public string Semester { get; set; }
    }
}
