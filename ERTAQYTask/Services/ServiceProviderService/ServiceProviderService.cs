using AutoMapper;
using BLLProject.UnitOfWorkPattern;
using DALProject.Entities;
using PLProject.ViewModel.ServiceProviderViewModel;

namespace PLProject.Services.ServiceProviderService
{
    public class ServiceProviderService(IUnitOfWork _unitOfWork, IMapper _mapper) : IServiceProviderService
    {
        public async Task<DetailServiceProviderViewModel> CreateServiceProviderAsync(CreateServiceProviderViewModel serviceProviderViewModel)
        {
            // Check if email already exists
            var emailExists = await _unitOfWork.ServiceProviders.EmailExistsAsync(serviceProviderViewModel.Email);
            if (emailExists)
            {
                throw new InvalidOperationException($"Email {serviceProviderViewModel.Email} already exists.");
            }

            var serviceProvider = _mapper.Map<DALProject.Entities.ServiceProvider>(serviceProviderViewModel);
            serviceProvider.CreatedDate = DateTime.Now;
            await _unitOfWork.ServiceProviders.CreateAsync(serviceProvider);
            return _mapper.Map<DetailServiceProviderViewModel>(serviceProvider);
        }

        public async Task DeleteServiceProviderAsync(int id)
        {
            var serviceProvider = await _unitOfWork.ServiceProviders.GetByIdAsync(id);
            if (serviceProvider != null)
            {
                await _unitOfWork.ServiceProviders.DeleteAsync(id);
            }
        }

        public async Task<IEnumerable<GetAllServiceProviderViewModel>> GetAllServiceProvidersAsync()
        {
            var serviceProviders = await _unitOfWork.ServiceProviders.GetAllAsync();
            return _mapper.Map<IEnumerable<GetAllServiceProviderViewModel>>(serviceProviders);
        }

        public async Task<DetailServiceProviderViewModel?> GetServiceProviderByIdAsync(int id)
        {
            var serviceProvider = await _unitOfWork.ServiceProviders.GetByIdAsync(id);
            if (serviceProvider == null)
            {
                return null;
            }
            return _mapper.Map<DetailServiceProviderViewModel>(serviceProvider);
        }

        public async Task UpdateServiceProviderAsync(DetailServiceProviderViewModel serviceProviderViewModel)
        {
            var existingServiceProvider = await _unitOfWork.ServiceProviders.GetByIdAsync(serviceProviderViewModel.Id);
            if (existingServiceProvider == null)
            {
                throw new InvalidOperationException($"ServiceProvider with ID {serviceProviderViewModel.Id} not found.");
            }

            // Check if email is being changed and if new email already exists
            if (existingServiceProvider.Email != serviceProviderViewModel.Email)
            {
                var emailExists = await _unitOfWork.ServiceProviders.EmailExistsAsync(serviceProviderViewModel.Email);
                if (emailExists)
                {
                    throw new InvalidOperationException($"Email {serviceProviderViewModel.Email} already exists.");
                }
            }

            _mapper.Map(serviceProviderViewModel, existingServiceProvider);
            await _unitOfWork.ServiceProviders.UpdateAsync(existingServiceProvider);
        }
    }
}
