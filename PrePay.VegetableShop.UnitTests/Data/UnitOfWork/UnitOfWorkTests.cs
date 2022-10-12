using Moq;
using Xunit;
using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Data.UnitOfWork;

namespace PrePay.VegetableShop.UnitTests.Data.UnitOfWork
{
    public class UnitOfWorkTests
    {
        private readonly Mock<ProductContext> productContext;
        private readonly ProductUnitOfWork _sut;

        public UnitOfWorkTests()
        {
            productContext = new Mock<ProductContext>();
            _sut = new ProductUnitOfWork(productContext.Object);
        }
    }
}
