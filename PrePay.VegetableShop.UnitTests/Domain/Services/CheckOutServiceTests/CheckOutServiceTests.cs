using FluentAssertions;
using Moq;
using PrePay.VegetableShop.Domain.Services.CheckoutService;
using PrePay.VegetableShop.Domain.Services.ProductService;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PrePay.VegetableShop.UnitTests.Domain.Services.CheckOutServiceTests
{
    public class CheckOutServiceTests
    {
        private readonly Mock<IProductService> _productService;
        private readonly CheckOutService _sut;

        public CheckOutServiceTests()
        {
            _productService = new Mock<IProductService>();
            _sut = new CheckOutService(_productService.Object);
        }

        [Fact]
        public async void CreateCheckOut_NothingToCheckOut_ShouldReturnEmpty()
        {
            //Arrange
            _productService.Setup(x => x.GetProducts()).ReturnsAsync(new List<Product>());

            //Act
            var result = await _sut.CheckOutProducts(new List<ProductOrder>());
            var productOrders = await result.ProductsPurchased.ToListAsync();

            result.TotalPrice.Should().Be(0);
            productOrders.Count.Should().Be(0);
        }

        [Fact]
        public async void CreateCheckOut_CalculatesTotalPrice_()
        {
            //Arrange
            var product = new List<Product>
            {
                new Product
                {
                    ProductName = ProductEnum.Tomato,
                    Price = 1f
                }
            };

            _productService.Setup(x => x.GetProducts()).ReturnsAsync(product);

            var productOrder = new List<ProductOrder>
                {
                    new ProductOrder
                    {
                        ProductName = ProductEnum.Tomato,
                        Quantity = 1
                    }
                };

            //Act
            var result = await _sut.CheckOutProducts(productOrder);
            var productOrders = await result.ProductsPurchased.ToListAsync();

            result.TotalPrice.Should().Be(1);
            productOrders.Count.Should().Be(1);
            productOrders[0].ProductName.Should().Be(ProductEnum.Tomato);
            productOrders[0].Quantity.Should().Be(1);
        }

        [Fact]
        public async void CreateCheckOut_AppliesAubergineDiscountAndCalculatesTotalPrice_()
        {
            //Arrange
            var product = new List<Product>
            {
                new Product
                {
                    ProductName = ProductEnum.Aubergine,
                    Price = 1f
                }
            };

            _productService.Setup(x => x.GetProducts()).ReturnsAsync(product);

            var productOrder = new List<ProductOrder>
                {
                    new ProductOrder
                    {
                        ProductName = ProductEnum.Aubergine,
                        Quantity = 3
                    }
                };

            //Act
            var result = await _sut.CheckOutProducts(productOrder);
            var productOrders = await result.ProductsPurchased.ToListAsync();

            result.TotalPrice.Should().Be(2);
            productOrders.Count.Should().Be(1);
            productOrders[0].ProductName.Should().Be(ProductEnum.Aubergine);
            productOrders[0].Quantity.Should().Be(3);
        }

        [Fact]
        public async void CreateCheckOut_NotEnoughTomatoes_DoesNotAppliesTomatoDealAndCalcuatesPrice()
        {
            //Arrange
            var products = new List<Product>
            {
                new Product
                {
                    ProductName = ProductEnum.Tomato,
                    Price = 0.1f
                },
                new Product
                {
                    ProductName = ProductEnum.Aubergine,
                    Price = 1f
                }
            };

            _productService.Setup(x => x.GetProducts()).ReturnsAsync(products);

            var productOrder = new List<ProductOrder>
                {
                    new ProductOrder()
                    {
                        ProductName = ProductEnum.Tomato,
                        Quantity = 1
                    }
                };

            //Act
            var result = await _sut.CheckOutProducts(productOrder);
            var productOrders = await result.ProductsPurchased.ToListAsync();

            result.TotalPrice.Should().Be(0.1f);
            productOrders.Count.Should().Be(1);
            productOrders[0].ProductName.Should().Be(ProductEnum.Tomato);
            productOrders[0].Quantity.Should().Be(1);
        }

        [Fact]
        public async void CreateCheckOut_NoAubergines_AppliesTomatoDealAndCalculatePrice()
        {
            //Arrange
            var products = new List<Product>
            {
                new Product
                {
                    ProductName = ProductEnum.Tomato,
                    Price = 0.1f
                },
                new Product
                {
                    ProductName = ProductEnum.Aubergine,
                    Price = 1f
                }
            };

            _productService.Setup(x => x.GetProducts()).ReturnsAsync(products);

            var productOrder = new List<ProductOrder>
                {
                    new ProductOrder
                    {
                        ProductName = ProductEnum.Tomato,
                        Quantity = 3
                    }
                };

            //Act
            var result = await _sut.CheckOutProducts(productOrder);
            var productOrders = await result.ProductsPurchased.ToListAsync();

            result.TotalPrice.Should().Be(0.3f);
            productOrders.Count.Should().Be(2);
            productOrders[0].ProductName.Should().Be(ProductEnum.Tomato);
            productOrders[0].Quantity.Should().Be(3);
            productOrders[1].ProductName.Should().Be(ProductEnum.Aubergine);
            productOrders[1].Quantity.Should().Be(1);
        }

        [Fact]
        public async void CreateCheckOut_Aubergines_AppliesTomatoDealAndCalcuatesPrice()
        {
            //Arrange
            var products = new List<Product>
            {
                new Product
                {
                    ProductName = ProductEnum.Tomato,
                    Price = 0.1f
                },
                new Product
                {
                    ProductName = ProductEnum.Aubergine,
                    Price = 1f
                }
            };

            _productService.Setup(x => x.GetProducts()).ReturnsAsync(products);

            var productOrder = new List<ProductOrder>
                {
                    new ProductOrder
                    {
                        ProductName = ProductEnum.Tomato,
                        Quantity = 3
                    },
                    new ProductOrder
                    {
                        ProductName = ProductEnum.Aubergine,
                        Quantity = 1
                    }
                }
                ;
            //Act
            var result = await _sut.CheckOutProducts(productOrder);
            var productOrders = await result.ProductsPurchased.ToListAsync();

            result.TotalPrice.Should().Be(1.3f);
            productOrders.Count.Should().Be(2);
            productOrders[0].ProductName.Should().Be(ProductEnum.Tomato);
            productOrders[0].Quantity.Should().Be(3);
            productOrders[1].ProductName.Should().Be(ProductEnum.Aubergine);
            productOrders[1].Quantity.Should().Be(2);
        }

        [Fact]
        public async void CreateCheckOut_Tomatoes_AppliesTomatoDiscountAndCalcuatesPrice()
        {
            //Arrange
            var products = new List<Product>
            {
                new Product
                {
                    ProductName = ProductEnum.Tomato,
                    Price = 1f
                },
                new Product
                {
                    ProductName = ProductEnum.Aubergine,
                    Price = 1f
                }
            };

            _productService.Setup(x => x.GetProducts()).ReturnsAsync(products);

            var productOrder = new List<ProductOrder>
                {
                    new ProductOrder
                    {
                        ProductName = ProductEnum.Tomato,
                        Quantity = 4
                    },
                    new ProductOrder
                    {
                        ProductName = ProductEnum.Aubergine,
                        Quantity = 1
                    }
                };

            //Act
            var result = await _sut.CheckOutProducts(productOrder);
            var productOrders = await result.ProductsPurchased.ToListAsync();

            result.TotalPrice.Should().Be(4f);
            productOrders.Count.Should().Be(2);
            productOrders[0].ProductName.Should().Be(ProductEnum.Tomato);
            productOrders[0].Quantity.Should().Be(4);
            productOrders[1].ProductName.Should().Be(ProductEnum.Aubergine);
            productOrders[1].Quantity.Should().Be(3);
        }
    }
}
