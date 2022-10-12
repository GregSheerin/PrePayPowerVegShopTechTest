using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Domain.Services.ProductsService
{
    public interface IProductService
    {
        public Task<Product> GetProduct(string productName);
        public Task<List<Product>> GetProducts();
    }
}
