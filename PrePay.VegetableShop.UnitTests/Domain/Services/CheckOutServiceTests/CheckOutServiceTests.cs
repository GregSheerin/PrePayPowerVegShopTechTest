using FluentAssertions;
using Moq;
using PrePay.VegetableShop.Domain.Services.CheckoutService;
using PrePay.VegetableShop.Domain.Services.ProductService;
using PrePay.VegetableShop.Models;
using System.Collections.Generic;
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
        public async void CreateCheckOut_NothingToCheckOut_ShouldReturnEmptyRecepit()
        {
            //Arrange
            _productService.Setup(x => x.GetProducts()).ReturnsAsync(new List<Product>());

            //Act
            var result = await _sut.CheckOutProducts(new List<ProductOrder>());

            //Assert
            result.FinalPrice.Should().Be(0);
            result.PurchasedProducts.Should().HaveCount(0);
            result.AllProducts.Should().HaveCount(0);
            result.OffersApplied.Should().HaveCount(0);
        }

        [Fact]
        public async void CreateCheckOut_CalculatesTotalPrice()
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

            //Assert
            result.FinalPrice.Should().Be(1);
            result.AllProducts.Should().HaveCount(1);
            result.PurchasedProducts.Should().HaveCount(1);
            result.AllProducts[0].ProductName.Should().Be(ProductEnum.Tomato);
            result.AllProducts[0].Quantity.Should().Be(1);
            result.PurchasedProducts[0].ProductName.Should().Be(ProductEnum.Tomato);
            result.PurchasedProducts[0].Quantity.Should().Be(1);
            result.OffersApplied.Should().BeEmpty();
        }

        [Fact]
        public async void CreateCheckOut_AppliesAubergineDiscountAndCalculatesTotalPrice()
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

            //Assert
            result.FinalPrice.Should().Be(2);
            result.AllProducts.Should().HaveCount(1);
            result.AllProducts[0].ProductName.Should().Be(ProductEnum.Aubergine);
            result.AllProducts[0].Quantity.Should().Be(3);
            result.PurchasedProducts.Should().HaveCount(1);
            result.PurchasedProducts[0].ProductName.Should().Be(ProductEnum.Aubergine);
            result.PurchasedProducts[0].Quantity.Should().Be(3);
            result.OffersApplied.Should().HaveCount(1);
            result.OffersApplied[0].Should().Be("You bought 3 Aubergine, so you get a total of 1 deducted from your total");
        }

        [Fact]
        public async void CreateCheckOut_NotEnoughTomatoes_DoesNotAppliesTomatoDealAndCalculatesPrice()
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

            //Assert
            result.FinalPrice.Should().Be(0.1f);
            result.AllProducts.Should().HaveCount(1);
            result.AllProducts[0].ProductName.Should().Be(ProductEnum.Tomato);
            result.AllProducts[0].Quantity.Should().Be(1);
            result.PurchasedProducts.Should().HaveCount(1);
            result.PurchasedProducts[0].ProductName.Should().Be(ProductEnum.Tomato);
            result.PurchasedProducts[0].Quantity.Should().Be(1);
            result.OffersApplied.Should().BeEmpty();
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

            //Assert
            result.FinalPrice.Should().Be(0.3f);
            result.AllProducts.Should().HaveCount(2);
            result.AllProducts[1].ProductName.Should().Be(ProductEnum.Tomato);
            result.AllProducts[1].Quantity.Should().Be(3);
            result.AllProducts[0].ProductName.Should().Be(ProductEnum.Aubergine);
            result.AllProducts[0].Quantity.Should().Be(1);
            result.PurchasedProducts.Should().HaveCount(1);
            result.PurchasedProducts[0].ProductName.Should().Be(ProductEnum.Tomato);
            result.PurchasedProducts[0].Quantity.Should().Be(3);
            result.OffersApplied.Should().HaveCount(1);
            result.OffersApplied[0].Should().Be("Because you bought 3 Tomato, you get 1 Aubergine for free!");
        }

        [Fact]
        public async void CreateCheckOut_Aubergines_AppliesTomatoDealAndCalculatesPrice()
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

            //Assert
            result.FinalPrice.Should().Be(1.3f);
            result.AllProducts.Should().HaveCount(2);
            result.AllProducts[1].ProductName.Should().Be(ProductEnum.Tomato);
            result.AllProducts[1].Quantity.Should().Be(3);
            result.AllProducts[0].ProductName.Should().Be(ProductEnum.Aubergine);
            result.AllProducts[0].Quantity.Should().Be(2);
            result.PurchasedProducts.Should().HaveCount(2);
            result.PurchasedProducts[0].ProductName.Should().Be(ProductEnum.Tomato);
            result.PurchasedProducts[0].Quantity.Should().Be(3);
            result.PurchasedProducts[1].ProductName.Should().Be(ProductEnum.Aubergine);
            result.PurchasedProducts[1].Quantity.Should().Be(1);
            result.OffersApplied.Should().HaveCount(2);
            result.OffersApplied[0].Should().Be("You bought 1 Aubergine, so you get a total of 0 deducted from your total");
            result.OffersApplied[1].Should().Be("Because you bought 3 Tomato, you get 1 Aubergine for free!");
        }

        [Fact]
        public async void CreateCheckOut_Tomatoes_AppliesTomatoDiscountAndCalculatesPrice()
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

            //Assert
            result.FinalPrice.Should().Be(4f);
            result.AllProducts.Should().HaveCount(2);
            result.AllProducts[1].ProductName.Should().Be(ProductEnum.Tomato);
            result.AllProducts[1].Quantity.Should().Be(4);
            result.AllProducts[0].ProductName.Should().Be(ProductEnum.Aubergine);
            result.AllProducts[0].Quantity.Should().Be(3);
            result.PurchasedProducts.Should().HaveCount(2);
            result.PurchasedProducts[0].ProductName.Should().Be(ProductEnum.Tomato);
            result.PurchasedProducts[0].Quantity.Should().Be(4);
            result.PurchasedProducts[1].ProductName.Should().Be(ProductEnum.Aubergine);
            result.PurchasedProducts[1].Quantity.Should().Be(1);
            result.OffersApplied.Should().HaveCount(3);
            result.OffersApplied[0].Should().Be("You bought 1 Aubergine, so you get a total of 0 deducted from your total");
            result.OffersApplied[1].Should().Be("Because you bought 4 Tomato, you get 2 Aubergine for free!");
            result.OffersApplied[2].Should().Be("Because you bought a total of 4 in Tomato, you get 1 deducted from your total!");
        }
    }
}
