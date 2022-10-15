using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PrePay.VegetableShop.Data.CsvParser;
using PrePay.VegetableShop.Models;
using System.IO;
using System.Linq;
using Xunit;

namespace PrePay.VegetableShop.UnitTests.Domain.Services.CsvMappers
{
    public class ProductOrderCsvParserTests
    {
        private readonly ProductOrderCsvParser _sut;

        public ProductOrderCsvParserTests()
        {
            _sut = new ProductOrderCsvParser();
        }

        [Fact]
        public async void ProductCsvParser_ShouldParseStreamToCsv()
        {
            //Arrange
            var streamReader = new StreamReader($"{Directory.GetParent(typeof(Program).Assembly.Location).FullName}\\ProductOrder.csv");

            //Act
            var result = await _sut.ParseCsv(streamReader);

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(4);

            result.Should().Contain(x => x.ProductName == ProductEnum.Aubergine);
            result.First(x => x.ProductName == ProductEnum.Aubergine).Quantity.Should().Be(25);

            result.Should().Contain(x => x.ProductName == ProductEnum.Tomato);
            result.First(x => x.ProductName == ProductEnum.Tomato).Quantity.Should().Be(3);

            result.Should().Contain(x => x.ProductName == ProductEnum.Carrot);
            result.First(x => x.ProductName == ProductEnum.Carrot).Quantity.Should().Be(12);

            result.Should().Contain(x => x.ProductName == ProductEnum.Potato);
            result.First(x => x.ProductName == ProductEnum.Potato).Quantity.Should().Be(10);
        }
    }
}
