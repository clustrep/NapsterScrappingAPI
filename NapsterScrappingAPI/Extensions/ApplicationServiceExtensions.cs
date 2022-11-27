using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NapsterScrappingBusiness.Gender;
using NapsterScrappingBusiness.Interfaces;
using NapsterScrappingInfrastructure.Cache;
using NapsterScrappingInfrastructure.Database.Gender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NapsterScrappingAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddMediatR(typeof(GendersSubGenresList.Handler).Assembly);
            services.AddMemoryCache();
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddScoped<IGenderRepository, GenderRepository>();
            return services;
        }
    }
}
