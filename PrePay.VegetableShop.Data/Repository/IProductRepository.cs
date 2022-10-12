using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Data.Repository
{
    public interface IProductRepository
    {
        public Task<Product> GetProduct(string productName); //TODO : Have a thing about encapsulating an id field so we dont have to do this via strings
        public Task<List<Product>> GetProducts();

        public Task SetProducts(List<Product> products); //TODO Have a think about how to inital the list, and if it needs to change or not
    }
}
