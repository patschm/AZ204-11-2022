using System.Threading.Tasks;
using Xunit;

namespace ACME.Tests.Integration
{
    public class CalculatorTests: IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;

        public CalculatorTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestIndexWithoutModel()
        {
            var client = _factory.CreateClient();
            var respose = await client.GetAsync("/calculator/index");

            Assert.NotNull(respose);
            Assert.True(respose.IsSuccessStatusCode);
        }
    }
}
