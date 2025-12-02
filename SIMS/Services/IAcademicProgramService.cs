using SIMS.Models;

namespace SIMS.Services
{
    public interface IAcademicProgramService
    {
        Task<List<AcademicProgram>> GetAllAsync();
    }
}