using System.Collections.Generic;
using System.Threading.Tasks;
using PrePay.VegetableShop.Models;

namespace PrePay.VegetableShop.Data.Context
{
    public interface IProductContext
    {
        public Task<Product> GetProduct(int productId);
        public Task<List<Product>> GetProducts();

        public void SetProducts();
    }
}
