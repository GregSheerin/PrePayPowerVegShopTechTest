using Microsoft.EntityFrameworkCore;
using PrePay.VegetableShop.Models;

namespace PrePay.VegetableShop.Data.Context
{
    public class ProductContext : DbContext, IProductContext
    {
        public ProductContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "ProductsDB");
        }

        public DbSet<Product> Products { get; set; }
    }
}
