using Microsoft.EntityFrameworkCore;
using PrePay.VegetableShop.Data.CsvParser;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading.Tasks;

namespace PrePay.VegetableShop.Data.Context
{
    public class ProductContext : DbContext, IProductContext
    {
        private readonly ICsvParser<Product> _csvParser;
        private readonly IFileSystem _fileSystem;
        public ProductContext(ICsvParser<Product> csvParser, DbContextOptions<ProductContext>
            options, IFileSystem fileSystem) : base(options)
        {
            _csvParser = csvParser;
            _fileSystem = fileSystem;
        }

        private DbSet<Product> Products { get; set; }

        public async Task<Product> GetProduct(int productId)
        {
            return await Products.FirstOrDefaultAsync(x => (int)x.ProductName == productId);
        }

        public async Task<List<Product>> GetProducts()
        {
            return await Products.ToListAsync();
        }

        public void SetProducts()
        {
            //TODO : Move this somewhere a bit more senseable, just need to load the data once
            var path = $"{_fileSystem.Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}\\ProductData.Csv";

            using var streamReader = new StreamReader(path);
            var products = _csvParser.ParseCsv(streamReader).Result;

            Products.AddRange(products);
            SaveChanges();
        }
    }
}
