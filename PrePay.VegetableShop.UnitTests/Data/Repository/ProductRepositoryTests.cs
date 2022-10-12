using Moq;
using Xunit;
using PrePay.VegetableShop.Data.Context;
using PrePay.VegetableShop.Data.Repository;


namespace PrePay.VegetableShop.UnitTests.Data.Repository
{
    public class ProductRepositoryTests
    {
        private readonly ProductRepository _sut;
        private readonly Mock<IProductContext> productContext;

        public ProductRepositoryTests()
        {
            productContext = new Mock<IProductContext>();
            _sut = new ProductRepository(productContext.Object);
        }
    }
}
