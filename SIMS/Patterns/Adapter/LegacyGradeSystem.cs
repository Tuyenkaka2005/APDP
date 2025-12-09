using System.Collections.Generic;

namespace SIMS.Patterns.Adapter
{
    // Adaptee: The legacy or external system
    public class LegacyGradeSystem
    {
        public List<string> GetLegacyTranscript(string studentId)
        {
            // Simulating data from a legacy system (e.g., CSV or old DB format)
            // Format: "SubjectCode|SubjectName|Score|Semester"
            return new List<string>
            {
                "HIS101|Vietnam History|8.5|Fall 2023",
                "PHI101|Introduction to Philosophy|7.0|Fall 2023",
                "PE001|Physical Education|9.0|Summer 2023"
            };
        }
    }
}
