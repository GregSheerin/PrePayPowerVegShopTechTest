using Microsoft.EntityFrameworkCore;
using PrePay.VegetableShop.Models;

namespace PrePay.VegetableShop.Data.Context
{
    public interface IProductContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
