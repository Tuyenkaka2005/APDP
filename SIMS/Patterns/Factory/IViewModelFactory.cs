using SIMS.Models.Entities;
using SIMS.Models.ViewModels;
using System.Threading.Tasks;

namespace SIMS.Patterns.Factory
{
    // Factory Method Interface
    public interface IViewModelFactory
    {
        Task<object> CreateViewModelAsync(Student student, string type);
    }
}
