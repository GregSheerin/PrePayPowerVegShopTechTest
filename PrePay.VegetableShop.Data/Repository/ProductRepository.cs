using Microsoft.EntityFrameworkCore;
using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Data.Repository
{
    public class ProductRepository : IProductRepository //TODO : Have a think about using a generic repo instead, not needed but logic here is simple
    {
        private readonly IProductContext _productContext;

        public ProductRepository(IProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task<Product> GetProduct(string productName)
        {
            return await _productContext.Products.FirstOrDefaultAsync(product => product.Name == productName);
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _productContext.Products.ToListAsync();
        }

        public async Task SetProducts(List<Product> products)
        {
            await _productContext.Products.AddRangeAsync(products); //TODO : Refactor this, once account for dupilcates, not exactly set
        }
    }
}
