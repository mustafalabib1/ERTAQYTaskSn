using AutoMapper;
using DALProject.Entities;
using PLProject.ViewModel.ServiceProviderViewModel;

namespace PLProject.Maping
{
    public class ServiceProviderProfile : Profile
    {
        public ServiceProviderProfile()
        {
            CreateMap<DALProject.Entities.ServiceProvider, GetAllServiceProviderViewModel>();
            CreateMap<DALProject.Entities.ServiceProvider, DetailServiceProviderViewModel>();
            CreateMap<CreateServiceProviderViewModel, DALProject.Entities.ServiceProvider>();
            CreateMap<DetailServiceProviderViewModel, DALProject.Entities.ServiceProvider>();
        }
    }
}
