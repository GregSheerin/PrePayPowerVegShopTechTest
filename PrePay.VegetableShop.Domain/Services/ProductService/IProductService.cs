using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Domain.Services.ProductService
{
    public interface IProductService
    {
        public Task<List<Product>> GetProducts();
    }
}
