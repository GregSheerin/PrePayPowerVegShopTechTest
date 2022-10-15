using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PrePay.VegetableShop.Data.CsvParser;
using PrePay.VegetableShop.Models;
using System.IO;
using System.Linq;
using Xunit;

namespace PrePay.VegetableShop.UnitTests.Domain.Services.CsvMappers
{
    public class ProductCsvParserTests
    {
        private readonly ProductCsvParser _sut;

        public ProductCsvParserTests()
        {
            _sut = new ProductCsvParser();
        }

        [Fact]
        public async void ProductCsvParser_ShouldParseStreamToCsv()
        {
            //Arrange
            var streamReader = new StreamReader($"{Directory.GetParent(typeof(Program).Assembly.Location).FullName}\\ProductData.csv");

            //Act
            var result = await _sut.ParseCsv(streamReader);

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(4);

            result.Should().Contain(x => x.ProductName == ProductEnum.Aubergine);
            result.First(x => x.ProductName == ProductEnum.Aubergine).Price.Should().Be(0.9f);

            result.Should().Contain(x => x.ProductName == ProductEnum.Tomato);
            result.First(x => x.ProductName == ProductEnum.Tomato).Price.Should().Be(0.75f);

            result.Should().Contain(x => x.ProductName == ProductEnum.Carrot);
            result.First(x => x.ProductName == ProductEnum.Carrot).Price.Should().Be(1f);

            result.Should().Contain(x => x.ProductName == ProductEnum.Potato);
            result.First(x => x.ProductName == ProductEnum.Potato).Price.Should().Be(0.33f);
        }
    }
}
