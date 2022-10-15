using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Data.Context
{
    public interface IProductContext
    {
        public Task<Product> GetProduct(int productId);
        public Task<List<Product>> GetProducts();

        public void SetProducts();
    }
}
