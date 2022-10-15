using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using AutoBogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using PrePay.VegetableShop.Models;
using PrePay.VegetableShop.Domain.Services.CheckoutService;
using PrePay.VegetableShop.API.Controllers;
using PrePay.VegetableShop.Data.CsvParser;

namespace PrePay.VegetableShop.UnitTests.Api.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<HttpContext> _httpContext;
        private readonly ProductsController _sut;
        private readonly Mock<ICheckOutService> _checkOutService;
        private readonly Mock<ICsvParser<ProductOrder>> _csvParser;

        public ProductsControllerTests()
        {
            _csvParser = new Mock<ICsvParser<ProductOrder>>();
            _checkOutService = new Mock<ICheckOutService>();
            _sut = new ProductsController(_checkOutService.Object, _csvParser.Object);
            _httpContext = new Mock<HttpContext>();
            _sut.ControllerContext.HttpContext = _httpContext.Object;
        }

        [Fact]
        public void ProductsController_ShouldImplementController()
        {
            //Assert
            typeof(ProductsController).Should().BeAssignableTo<Controller>();
        }

        [Fact]
        public void CheckOutOrder_ShouldHaveHttpPostAttribute()
        {
            //Assert
            typeof(ProductsController).GetMethod("CheckOutOrder")
                ?.GetCustomAttributes(typeof(HttpPostAttribute), false)
                .Should().BeAssignableTo(typeof(HttpPostAttribute[])).And.HaveCount(1);
        }

        [Fact]
        public async void CheckOutOrder_NullProductOrder_Returns400BadRequest()
        {
            //Arrange
            _httpContext.Setup(x => x.Request.Body.Seek(0, SeekOrigin.Begin));

            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("TestData"));
            _httpContext.Setup(x => x.Request.Body).Returns(stream);

            var streamReader = new StreamReader(stream);
            _csvParser.Setup(x => x.ParseCsv(streamReader)).ReturnsAsync((List<ProductOrder>)null);

            //Act
            var res = await _sut.CheckOutOrder(null).ConfigureAwait(false);

            //Assert
            res.Should().NotBeNull();
            var badRequest = (BadRequestObjectResult)res.Result;
            badRequest.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequest.Value.Should().Be("No Product Data");
        }

        [Fact]
        public async void CheckOutOrder_EmptyProductOrder_Returns400BadRequest()
        {
            //Arrange
            _httpContext.Setup(x => x.Request.Body.Seek(0, SeekOrigin.Begin));

            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("TestData"));
            _httpContext.Setup(x => x.Request.Body).Returns(stream);

            var streamReader = new StreamReader(stream);
            var emptyProdList = new List<ProductOrder>();
            _csvParser.Setup(x => x.ParseCsv(streamReader)).ReturnsAsync(emptyProdList);

            //Act
            var res = await _sut.CheckOutOrder(null).ConfigureAwait(false);

            //Assert
            res.Should().NotBeNull();
            var badRequest = (BadRequestObjectResult)res.Result;
            badRequest.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequest.Value.Should().Be("No Product Data");
        }

        [Fact]
        public async void CheckOutOrder_ProductsProvided_Returns200WithCheckout()
        {
            //Arrange
            _httpContext.Setup(x => x.Request.Body.Seek(0, SeekOrigin.Begin));

            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("TestData"));
            _httpContext.Setup(x => x.Request.Body).Returns(stream);

            var streamReader = new StreamReader(stream);
            var productOrders = AutoFaker.Generate<ProductOrder>(2);
            _csvParser.Setup(x => x.ParseCsv(It.IsAny<StreamReader>())).ReturnsAsync(productOrders);

            var checkOut = AutoFaker.Generate<CheckOut>();
            _checkOutService.Setup(x => x.CheckOutProducts(productOrders)).ReturnsAsync(checkOut);

            //Act
            var res = await _sut.CheckOutOrder(null).ConfigureAwait(false);

            //Assert
            res.Should().NotBeNull();
            var okObjectResult = (OkObjectResult)res.Result;
            okObjectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObjectResult.Value.Should().Be(checkOut);
        }
    }
}
