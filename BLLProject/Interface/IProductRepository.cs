using DALProject.Entities;
using System.Data;
using System.Security.Cryptography;

namespace BLLProject.Interface
{
    public interface IProductRepository : IRepository<Product, int>
    {
        Task<IEnumerable<Product>> GetByServiceProviderIdAsync(int serviceProviderId);
        Task<IEnumerable<Product>> GetProductsAbovePriceAsync(decimal minPrice);
        Task<IEnumerable<Product>> GetProductsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Product>> GetFilteredProductsAsync(decimal? minPrice, decimal? maxPrice, DateTime? fromDate, DateTime? toDate, int? serviceProviderId);
        Task<bool> ServiceProviderExistsAsync(int serviceProviderId);
    }
}
