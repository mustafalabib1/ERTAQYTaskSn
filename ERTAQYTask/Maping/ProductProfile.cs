using AutoMapper;
using DALProject.Entities;
using PLProject.ViewModel.ProductViewModel;

namespace PLProject.Maping
{
    public class ProductProfile :Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, GetAllProductViewModel>()
                .ForMember(dest => dest.ServiceProviderName, opt => opt.MapFrom(src => src.ServiceProvider != null ? src.ServiceProvider.Name : null));
            CreateMap<Product, DetialProductViewModel>()
                .ForMember(dest => dest.ServiceProviderName, opt => opt.MapFrom(src => src.ServiceProvider != null ? src.ServiceProvider.Name : null));
            CreateMap<CreateProductViewModel, Product>();
        }
    }
}
