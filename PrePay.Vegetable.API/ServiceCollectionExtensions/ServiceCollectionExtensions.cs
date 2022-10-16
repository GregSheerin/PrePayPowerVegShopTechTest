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
        //Extension method to add on all services needed for the application
        public static void AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>()
                .AddTransient<ICsvParser<ProductOrder>, ProductOrderCsvParser>()
                .AddTransient<ICsvParser<Product>, ProductCsvParser>()
                .AddScoped<ICheckOutService, CheckOutService>()
                .AddScoped<IFileSystem, FileSystem>(); //System.Io.Abstractions, very useful when working with a filesystem(provides injection abstractions for the filesystem)
        }

        public static void AddDatabaseRepository(this IServiceCollection services)
        {
            services.AddDbContext<IProductContext, ProductContext>(opt => opt.UseInMemoryDatabase("ProductsDB"));

            //Spilt between keeping this here or making something else, only time the DB needs to be set up
            //The assignment didn't specify a database, I am using the in-memory database as a way to save reading and writing the csv file
            //Idea is that it is a static database loaded from the file, but with entity framework it would be easy to replace the mock in memeory database with a real one
            //More on this in the readme(Data handling section).
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IProductContext>();

            dbContext.SetProducts();
        }
    }
}
