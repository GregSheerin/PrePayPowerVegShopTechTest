using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Domain.Services.ProductService
{
    public class ProductService : IProductService
    {
        //I use an Interface over the context here, One it means that the database impletation doesnt affect the flow of the app
        //And it makes testings a lot simplier
        //For the operations(just), i stuck purely to what was needed for the assignment.
        //If we need more read/write/get style implentation, they would be implented here(Assuming a unit of work pattern+repoistry isnt added in)
        private readonly IProductContext _productContext;

        public ProductService(IProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _productContext.GetProducts();
        }
    }
}
