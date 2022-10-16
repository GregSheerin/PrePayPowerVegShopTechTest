using CsvHelper;
using PrePay.VegetableShop.Data.CsvParser.CsvMappers;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Data.CsvParser
{
    public class ProductCsvParser : ICsvParser<Product>
    {
       //Parsers in charge of reading the inital data file
       //Using CSV helper lib for this, Mapper profile converts the expected header to the correct property on the model
        public async Task<List<Product>> ParseCsv(StreamReader productStream)
        {
            using var csv = new CsvReader(productStream, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ProductMap>();

            var products = await csv.GetRecordsAsync<Product>().ToListAsync();
            return products;

        }
    }
}