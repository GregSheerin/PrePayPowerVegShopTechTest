using Microsoft.Extensions.DependencyInjection;
using PrePay.VegetableShop.Data.Repository;
using PrePay.VegetableShop.Data.UnitOfWork;
using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Domain.Services.ProductsService;
using PrePay.VegetableShop.Domain.Services.CheckoutService;

namespace PrePay.VegetableShop.API.IServiceCollectionExtensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICheckOutService, CheckOutService>();
        }

        public static void AddDatabaseRepoistory(this IServiceCollection services)
        {
            services.AddDbContext<IProductContext, ProductContext>();
            services.AddTransient<IUnitOfWork, ProductUnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
