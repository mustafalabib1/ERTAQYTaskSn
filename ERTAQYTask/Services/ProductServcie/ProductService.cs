using AutoMapper;
using BLLProject.UnitOfWorkPattern;
using DALProject.Entities;
using PLProject.ViewModel.ProductViewModel;
namespace PLProject.Services.ProductServcie
{
    public class ProductService(IUnitOfWork _unitOfWork, IMapper _mapper) : IProductService
    {
        public async Task<DetialProductViewModel> CreateProductAsync(CreateProductViewModel productViewModel)
        {
            try
            {
                // Validate ServiceProvider exists
                var serviceProviderExists = await _unitOfWork.Products.ServiceProviderExistsAsync(productViewModel.ServiceProviderId);
                if (!serviceProviderExists)
                {
                    throw new InvalidOperationException($"ServiceProvider with ID {productViewModel.ServiceProviderId} does not exist.");
                }

                var product = _mapper.Map<Product>(productViewModel);

                // Create product and get the generated ID
                var productId = await _unitOfWork.Products.CreateAsync(product);

                if (productId <= 0)
                {
                    throw new InvalidOperationException("Failed to create product. No ID was returned.");
                }

                // Now the product object should have the ID set, but let's refetch to be sure
                var createdProduct = await _unitOfWork.Products.GetByIdAsync(productId);

                if (createdProduct == null)
                {
                    throw new InvalidOperationException("Product was created but could not be retrieved.");
                }

                return _mapper.Map<DetialProductViewModel>(createdProduct);
            }
            catch (Exception ex)
            {
                // Log the error
                throw new InvalidOperationException($"Error creating product: {ex.Message}", ex);
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                await _unitOfWork.Products.DeleteAsync(id);
            }
        }

        public async Task<IEnumerable<GetAllProductViewModel>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return _mapper.Map<IEnumerable<GetAllProductViewModel>>(products);
        }

        public async Task<DetialProductViewModel?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            return _mapper.Map<DetialProductViewModel>(product);
        }

        public async Task UpdateProductAsync(DetialProductViewModel productViewModel)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(productViewModel.Id);
            if (existingProduct == null)
            {
                throw new InvalidOperationException($"Product with ID {productViewModel.Id} not found.");
            }

            // Validate ServiceProvider exists
            var serviceProviderExists = await _unitOfWork.Products.ServiceProviderExistsAsync(productViewModel.ServiceProviderId);
            if (!serviceProviderExists)
            {
                throw new InvalidOperationException($"ServiceProvider with ID {productViewModel.ServiceProviderId} does not exist.");
            }

            _mapper.Map(productViewModel, existingProduct);
            await _unitOfWork.Products.UpdateAsync(existingProduct);
        }

        // Additional methods using custom repository methods
        public async Task<IEnumerable<GetAllProductViewModel>> GetProductsByServiceProviderAsync(int serviceProviderId)
        {
            var products = await _unitOfWork.Products.GetByServiceProviderIdAsync(serviceProviderId);
            return _mapper.Map<IEnumerable<GetAllProductViewModel>>(products);
        }

        public async Task<IEnumerable<GetAllProductViewModel>> GetProductsAbovePriceAsync(decimal minPrice)
        {
            var products = await _unitOfWork.Products.GetProductsAbovePriceAsync(minPrice);
            return _mapper.Map<IEnumerable<GetAllProductViewModel>>(products);
        }

        public async Task<IEnumerable<GetAllProductViewModel>> GetFilteredProductsAsync(decimal? minPrice, decimal? maxPrice, DateTime? fromDate, DateTime? toDate, int? serviceProviderId)
        {
            var products = await _unitOfWork.Products.GetFilteredProductsAsync(minPrice, maxPrice, fromDate, toDate, serviceProviderId);
            return _mapper.Map<IEnumerable<GetAllProductViewModel>>(products);
        }
    }
}