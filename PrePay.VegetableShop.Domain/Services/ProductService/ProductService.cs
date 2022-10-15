using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
