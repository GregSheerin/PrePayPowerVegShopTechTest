using Moq;
using Xunit;
using AutoBogus;
using FluentAssertions;
using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Domain.Services.ProductService;
using PrePay.VegetableShop.Models;

namespace PrePay.VegetableShop.UnitTests.Domain.Services.ProductServiceTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductContext> _productContext;
        private readonly ProductService _sut;

        public ProductServiceTests()
        {
            _productContext = new Mock<IProductContext>();
            _sut = new ProductService(_productContext.Object);
        }

        [Fact]
        public async void GetProduct_ShouldReturnMatchingProduct()
        {
            //Arrange
            var product = AutoFaker.Generate<Product>();
            _productContext.Setup(x => x.GetProduct((int)product.ProductName)).ReturnsAsync(product);

            //Act
            var result = await _sut.GetProduct((int)product.ProductName).ConfigureAwait(false);
            result.Should().Be(product);
        }

        [Fact]
        public async void GetProducts_ShouldReturnAllProducts()
        {
            //Arrange
            var products = AutoFaker.Generate<Product>(5);
            _productContext.Setup(x => x.GetProducts()).ReturnsAsync(products);

            //Act
            var result = await _sut.GetProducts().ConfigureAwait(false);
            result.Should().BeEquivalentTo(products);
        }
    }
}
