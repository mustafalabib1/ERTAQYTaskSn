using DALProject.Entities;

namespace BLLProject.Interface
{
    public interface IServiceProviderRepository : IRepository<ServiceProvider, int>
    {
        Task<ServiceProvider?> GetByEmailAsync(string email);
        Task<IEnumerable<ServiceProvider>> SearchByNameAsync(string name);
        Task<bool> EmailExistsAsync(string email);
    }
}
