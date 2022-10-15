using CsvHelper.Configuration;
using PrePay.VegetableShop.Models;

namespace PrePay.VegetableShop.Data.CsvParser.CsvMappers
{
    public sealed class ProductMap : ClassMap<Product>
    {
        public ProductMap()
        {
            Map(m => m.ProductName).Name("PRODUCT");
            Map(m => m.Price).Name("PRICE");
        }
    }
}
