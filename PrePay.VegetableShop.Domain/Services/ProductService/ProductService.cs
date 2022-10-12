using System.Collections.Generic;
using System.Threading.Tasks;
using PrePay.VegetableShop.Data.UnitOfWork;
using PrePay.VegetableShop.Models;

namespace PrePay.VegetableShop.Domain.Services.ProductsService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> GetProduct(string productName)
        {
            return await _unitOfWork.Products.GetProduct(productName);
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _unitOfWork.Products.GetProducts();
        }
    }
}
