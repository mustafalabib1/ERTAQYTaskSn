using BLLProject.UnitOfWorkPattern;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLLProject
{
    public static class ModuleBLLDependenices
    {
        public static IServiceCollection AddBLLDependencies(this IServiceCollection services)
        {
            // Register BLL services and repositories here
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
