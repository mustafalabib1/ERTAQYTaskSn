using PLProject.Services.ProductServcie;
using PLProject.Services.ServiceProviderService;

namespace PLProject
{
    public static class ModulePLDependances
    {
        public static IServiceCollection AddPLDependances(this IServiceCollection services)
        {
            // Configure AutoMapper 
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ModulePLDependances).Assembly));
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IServiceProviderService, ServiceProviderService>();
            return services;
        }

    }
}
