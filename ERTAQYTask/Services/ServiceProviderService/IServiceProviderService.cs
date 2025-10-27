using PLProject.ViewModel.ServiceProviderViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PLProject.Services.ServiceProviderService
{
    public interface IServiceProviderService
    {
        Task<IEnumerable<GetAllServiceProviderViewModel>> GetAllServiceProvidersAsync();
        Task<DetailServiceProviderViewModel?> GetServiceProviderByIdAsync(int id);
        Task<DetailServiceProviderViewModel> CreateServiceProviderAsync(CreateServiceProviderViewModel serviceProviderViewModel);
        Task UpdateServiceProviderAsync(DetailServiceProviderViewModel serviceProviderViewModel);
        Task DeleteServiceProviderAsync(int id);
    }
}
