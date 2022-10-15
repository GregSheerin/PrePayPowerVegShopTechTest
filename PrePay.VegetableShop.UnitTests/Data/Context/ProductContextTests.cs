using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Data.CsvParser;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Xunit;

namespace PrePay.VegetableShop.UnitTests.Data.Context
{
    public class ProductContextTests
    {
        private readonly ProductContext _sut;
        private readonly Mock<IPath> _path;
        private readonly Mock<ICsvParser<Product>> _csvParser;

        public ProductContextTests()
        {
            var contextOptions = new DbContextOptionsBuilder<ProductContext>()
        .UseInMemoryDatabase("ProductUTDb")
        .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
        .Options;

            _path = new Mock<IPath>();
            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(x => x.Path).Returns(_path.Object);
            _csvParser = new Mock<ICsvParser<Product>>();

            _sut = new ProductContext(_csvParser.Object, contextOptions, fileSystem.Object);
            _sut.Database.EnsureDeleted();
            _sut.Database.EnsureCreated();
        }

        [Fact]
        public async void GetProduct_ProductPresent_ReturnsProduct()
        {
            //Arrange
            var currentPath = Directory.GetParent(typeof(Program).Assembly.Location).FullName;
            _path.Setup(x => x.GetDirectoryName(It.IsAny<string>()))
                .Returns(currentPath);

            var product = new List<Product>
            {
                new Product
                {
                    ProductName = ProductEnum.Aubergine, Price = 1f
                }
            };
            _csvParser.Setup(x => x.ParseCsv(It.IsAny<StreamReader>())).ReturnsAsync(product);

            _sut.SetProducts();

            //Act
            var result = await _sut.GetProduct(1).ConfigureAwait(false);

            //Assert
            result.ProductName.Should().Be(ProductEnum.Aubergine);
            result.Price.Should().Be(product[0].Price);
        }

        [Fact]
        public async void GetProduct_ProductNotPresent_ReturnsNull()
        {
            //Act
            var result = await _sut.GetProduct(1).ConfigureAwait(false);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async void GetProducts_ProductsPresent_ReturnsProduct()
        {
            //Arrange
            var currentPath = Directory.GetParent(typeof(Program).Assembly.Location).FullName;
            _path.Setup(x => x.GetDirectoryName(It.IsAny<string>()))
                .Returns(currentPath);

            var product = new List<Product> { new Product { ProductName = ProductEnum.Aubergine, Price = 1f } };
            _csvParser.Setup(x => x.ParseCsv(It.IsAny<StreamReader>())).ReturnsAsync(product);

            _sut.SetProducts();

            //Act
            var result = await _sut.GetProducts().ConfigureAwait(false);

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            result[0].ProductName.Should().Be(ProductEnum.Aubergine);
            result[0].Price.Should().Be(product[0].Price);
        }

        [Fact]
        public async void GetProducts_ProductNotPresent_ReturnsNull()
        {
            //Act
            var result = await _sut.GetProduct(1).ConfigureAwait(false);

            //Assert
            result.Should().BeNull();
        }
    }
}
