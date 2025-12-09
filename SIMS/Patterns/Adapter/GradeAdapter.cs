using System;
using System.Collections.Generic;
using System.Linq;

namespace SIMS.Patterns.Adapter
{
    // Adapter: Adapts LegacyGradeSystem to IExternalGradeSystem
    public class GradeAdapter : IExternalGradeSystem
    {
        private readonly LegacyGradeSystem _legacySystem;

        public GradeAdapter()
        {
            _legacySystem = new LegacyGradeSystem();
        }

        public List<ExternalGrade> GetGradesForStudent(string studentCode)
        {
            var legacyData = _legacySystem.GetLegacyTranscript(studentCode);
            var externalGrades = new List<ExternalGrade>();

            foreach (var line in legacyData)
            {
                var parts = line.Split('|');
                if (parts.Length == 4)
                {
                    externalGrades.Add(new ExternalGrade
                    {
                        SubjectCode = parts[0],
                        SubjectName = parts[1],
                        Score = double.Parse(parts[2]),
                        Semester = parts[3]
                    });
                }
            }

            return externalGrades;
        }
    }
}
