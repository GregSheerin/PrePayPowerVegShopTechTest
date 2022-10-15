using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CsvHelper;
using PrePay.VegetableShop.Models;
using PrePay.VegetableShop.Data.CsvParser.CsvMappers;

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
