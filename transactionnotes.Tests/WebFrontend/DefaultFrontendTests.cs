using transactionnotes.Tests.Common;

namespace transactionnotes.Tests.WebFrontend
{
    public class DefaultFrontendTests
    {
        public class DefaultTests() : HttpTestBase(WebFrontendResource)
        {
            [Fact]
            public async Task GetWebResourceRootReturnsOkStatusCode()
            {
                // Arrange 
                // (already done in InitializeAsync)

                // Act
                var response = await HttpClient.GetAsync("/");

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
