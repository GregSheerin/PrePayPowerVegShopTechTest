using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PrePay.VegetableShop.Data.Context;

namespace PrePay.VegetableShop.UnitTests.Data.Context
{
    public class ProductContextTests
    {
        private readonly ProductContext _sut;

        public ProductContextTests()
        {
            var contextOptions = new DbContextOptionsBuilder<ProductContext>()
        .UseInMemoryDatabase("ProductUTDb")
        .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;

            _sut = new ProductContext(contextOptions);
            _sut.Database.EnsureDeleted();
            _sut.Database.EnsureCreated();
        }
    }
}
