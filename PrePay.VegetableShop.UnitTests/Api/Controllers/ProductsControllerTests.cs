using Moq;
using Xunit;
using PrePay.Vegetable.API.Controllers;
using PrePay.VegetableShop.Domain.Services.CheckoutService;

namespace PrePay.VegetableShop.UnitTests.Api.Controllers
{
    public class ProductsControllerTests
    {
        private readonly ProductsController _sut;
        private readonly Mock<ICheckOutService> _checkOutService;
        public ProductsControllerTests()
        {
            _checkOutService = new Mock<ICheckOutService>();
            _sut = new ProductsController(_checkOutService.Object);
        }
    }
}
