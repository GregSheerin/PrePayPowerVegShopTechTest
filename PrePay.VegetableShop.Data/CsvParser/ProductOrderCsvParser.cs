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
    public class ProductOrderCsvParser : ICsvParser<ProductOrder>
    {
        public async Task<List<ProductOrder>> ParseCsv(StreamReader productStream)
        {
            using var csv = new CsvReader(productStream, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ProductOrderMap>();

            var productOrder = await csv.GetRecordsAsync<ProductOrder>().ToListAsync();
            return productOrder;
        }
    }
}
