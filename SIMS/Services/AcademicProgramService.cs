using Microsoft.EntityFrameworkCore;
using SIMS.Data;
using SIMS.Models;

namespace SIMS.Services
{
    public class AcademicProgramService : IAcademicProgramService
    {
        private readonly ApplicationDbContext _context;

        public AcademicProgramService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AcademicProgram>> GetAllAsync()
        {
            return await _context.AcademicPrograms
                .OrderBy(p => p.ProgramName)
                .ToListAsync();
        }
    }
}