using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using PrePay.VegetableShop.API.ServiceCollectionExtensions;
using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Data.CsvParser;
using PrePay.VegetableShop.Domain.Services.CheckoutService;
using PrePay.VegetableShop.Domain.Services.ProductService;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Xunit;

namespace PrePay.VegetableShop.UnitTests.Api.ServiceCollectionExtensions
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection _sut;

        public ServiceCollectionExtensionsTests()
        {
            _sut = new ServiceCollection();
        }

        [Fact]
        public void AddDomainServices_ShouldAddAllDomainServices()
        {
            //Act
            _sut.AddDomainServices();

            //Assert
            var productService = _sut.FirstOrDefault(e => e.ServiceType == typeof(IProductService));
            productService.Should().NotBeNull();
            productService.ServiceType.Should().BeAssignableTo<IProductService>();
            productService.ImplementationType.Should().BeAssignableTo<ProductService>();
            productService.Lifetime.Should().Be(ServiceLifetime.Scoped);

            var prodcutOrderCsvService = _sut.FirstOrDefault(e => e.ServiceType == typeof(ICsvParser<ProductOrder>));
            prodcutOrderCsvService.Should().NotBeNull();
            prodcutOrderCsvService.ServiceType.Should().BeAssignableTo<ICsvParser<ProductOrder>>();
            prodcutOrderCsvService.ImplementationType.Should().BeAssignableTo<ProductOrderCsvParser>();
            prodcutOrderCsvService.Lifetime.Should().Be(ServiceLifetime.Transient);

            var csvService = _sut.FirstOrDefault(e => e.ServiceType == typeof(ICsvParser<Product>));
            csvService.Should().NotBeNull();
            csvService.ServiceType.Should().BeAssignableTo<ICsvParser<Product>>();
            csvService.ImplementationType.Should().BeAssignableTo<ProductCsvParser>();
            csvService.Lifetime.Should().Be(ServiceLifetime.Transient);

            var checkOutService = _sut.FirstOrDefault(e => e.ServiceType == typeof(ICheckOutService));
            checkOutService.Should().NotBeNull();
            checkOutService.ServiceType.Should().BeAssignableTo<ICheckOutService>();
            checkOutService.ImplementationType.Should().BeAssignableTo<CheckOutService>();
            checkOutService.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }

        [Fact]
        public void AddDatabaseRepository_ShouldAddAllDbServices()
        {
            //Arrange
            var currentPath = Directory.GetParent(typeof(Program).Assembly.Location).FullName;

            var path = new Mock<IPath>();
            path.Setup(x => x.GetDirectoryName(It.IsAny<string>()))
                .Returns(currentPath);

            var product = new List<Product> { new Product { ProductName = ProductEnum.Aubergine, Price = 1f } };

            var csvParser = new Mock<ICsvParser<Product>>();
            csvParser.Setup(x => x.ParseCsv(It.IsAny<StreamReader>())).ReturnsAsync(product);

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(x => x.Path).Returns(path.Object);

            _sut.AddSingleton<ICsvParser<Product>>(csvParser.Object);
            _sut.AddSingleton<IFileSystem>(fileSystem.Object);

            //Act
            _sut.AddDatabaseRepository();

            //Assert
            var productContext = _sut.FirstOrDefault(e => e.ServiceType == typeof(IProductContext));
            productContext.Should().NotBeNull();
            productContext.ServiceType.Should().BeAssignableTo<IProductContext>();
            productContext.ImplementationType.Should().BeAssignableTo<ProductContext>();
            productContext.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }
    }
}
