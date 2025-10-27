using PLProject.ViewModel.ProductViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLProject.Services.ProductServcie
{
    public interface IProductService
    {
        Task<IEnumerable<GetAllProductViewModel>> GetAllProductsAsync();
        Task<DetialProductViewModel?> GetProductByIdAsync(int id);
        Task<DetialProductViewModel> CreateProductAsync(CreateProductViewModel productViewModel);
        Task UpdateProductAsync(DetialProductViewModel productViewModel);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<GetAllProductViewModel>> GetFilteredProductsAsync(decimal? minPrice, decimal? maxPrice, DateTime? fromDate, DateTime? toDate, int? serviceProviderId);
    }
}
