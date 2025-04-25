using transactionnotes.Tests.Common;

namespace transactionnotes.Tests.ApiService
{
    public class PingTests() : HttpTestBase(ApiServiceResource)
    {
        [Fact]
        public async Task GetWebResourceRootReturnsOkStatusCode()
        {
            // Arrange 
            // (already done in InitializeAsync)

            // Act
            var response = await HttpClient.GetAsync("/api/v1/ping");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
