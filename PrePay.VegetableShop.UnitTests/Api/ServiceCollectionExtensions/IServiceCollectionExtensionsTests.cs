using Moq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace PrePay.VegetableShop.UnitTests.Api.ServiceCollectionExtensions
{
    public class IServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection _sut;

        public IServiceCollectionExtensionsTests()
        {
            _sut = new ServiceCollection();
        }
    }
}
