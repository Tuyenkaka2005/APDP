using SIMS.Models;

namespace SIMS.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync(string? search = null, int? programId = null);
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByIdWithDetailsAsync(int id);
        Task CreateAsync(Student student, string password, string password1);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
        Task<bool> StudentCodeExistsAsync(string code, int? excludeId = null);
    }
}