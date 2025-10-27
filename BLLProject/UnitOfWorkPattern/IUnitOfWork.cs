using BLLProject.Interface;

namespace BLLProject.UnitOfWorkPattern
{
    public interface IUnitOfWork : IDisposable
    {
        IServiceProviderRepository ServiceProviders { get; }
        IProductRepository Products { get; }
        Task<int> SaveChangesAsync();
    }
}