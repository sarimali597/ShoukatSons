using Microsoft.Extensions.DependencyInjection;
using ShoukatSons.Core.Interfaces;
using ShoukatSons.Services.Implementations;

namespace ShoukatSons.Services
{
    public static class ServiceRegistry
    {
        public static IServiceCollection AddShoukatSonsServices(this IServiceCollection services)
        {
            // Interface-backed services that exist
            services.AddScoped<IProductService, ProductService>();

            // Register concrete services directly to avoid compile errors for missing interfaces.
            // If/when you add IStockService / ISalesService / IReturnExchangeService / ISecurityService
            // move these to interface-based registrations.
            services.AddScoped<StockService>();
            services.AddScoped<SalesServiceImpl>();
            services.AddScoped<ReturnExchangeService>();
            services.AddScoped<SecurityService>();

            return services;
        }
    }
}