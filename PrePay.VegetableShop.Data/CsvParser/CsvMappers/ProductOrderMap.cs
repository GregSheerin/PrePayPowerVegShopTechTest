using CsvHelper.Configuration;
using PrePay.VegetableShop.Models;

namespace PrePay.VegetableShop.Data.CsvParser.CsvMappers
{
    public sealed class ProductOrderMap : ClassMap<ProductOrder>
    {
        public ProductOrderMap()
        {
            Map(m => m.ProductName).Name("PRODUCT");
            Map(m => m.Quantity).Name("QUANTITY");
        }
    }
}
