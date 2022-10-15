using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.TypeConversion;
using PrePay.VegetableShop.Models;
using PrePay.VegetableShop.Data.CsvParser.CsvMappers;

namespace PrePay.VegetableShop.Data.CsvParser
{
    public class ProductCsvParser : ICsvParser<Product>
    {
        //Can assume that the system data is safe from error, but should put something simliar in there
        public async Task<List<Product>> ParseCsv(StreamReader productStream)
        {
            using var csv = new CsvReader(productStream, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ProductMap>();

            var products = await csv.GetRecordsAsync<Product>().ToListAsync();
            return products;

        }
    }
}