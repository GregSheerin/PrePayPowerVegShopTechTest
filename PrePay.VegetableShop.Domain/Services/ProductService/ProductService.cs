using System.Threading.Tasks;
using System.Collections.Generic;
using PrePay.VegetableShop.Models;
using PrePay.VegetableShop.Data.Context;

namespace PrePay.VegetableShop.Domain.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductContext _productContext;

        public ProductService(IProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task<Product> GetProduct(int productId)
        {
            return await _productContext.GetProduct(productId);
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _productContext.GetProducts();
        }
    }
}
