using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Data.CsvParser;
using PrePay.VegetableShop.Domain.Services.CheckoutService;
using PrePay.VegetableShop.Domain.Services.ProductService;
using PrePay.VegetableShop.Models;
using System.IO.Abstractions;

namespace PrePay.VegetableShop.API.ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>()
                .AddTransient<ICsvParser<ProductOrder>, ProductOrderCsvParser>()
                .AddTransient<ICsvParser<Product>, ProductCsvParser>()
                .AddScoped<ICheckOutService, CheckOutService>()
                .AddScoped<IFileSystem, FileSystem>();
        }

        public static void AddDatabaseRepository(this IServiceCollection services)
        {
            services.AddDbContext<IProductContext, ProductContext>(opt => opt.UseInMemoryDatabase("ProductsDB"));
            //Spilt between keeping this here or making something else, only time the DB needs to be set up, but try and remove it from here
            //TODO : try and see if theres a way to fire the set when the dependacy is added to container
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IProductContext>();

            dbContext.SetProducts();
        }
    }
}
