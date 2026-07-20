using Sol.Common;
using System.Net;

namespace Sol.Api.IntegrationTests.Controllers.V1;

public static class HealthControllerTests
{
    private const string BaseUrl = "api/health";
    private const string VersionedUrl = "api/v1/Health";
    
    public class Ping
    {
        [Fact]
        public async Task ShouldReturnNoContent_WhenCalledWithVersionHeader()
        {
            const HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent;
            
            await using var factory = new ApiFactory();
            using var client = factory.CreateClient();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, BaseUrl)
            {
                Headers =
                {
                    { Constants.ApiVersionHeaderKey, "1.0" }
                }
            };
            
            var response = await client.SendAsync(requestMessage, TestContext.Current.CancellationToken);
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnNoContent_WhenCalledWithVersionedUrl()
        {
            const HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent;
            
            await using var factory = new ApiFactory();
            using var client = factory.CreateClient();

            var response = await client.GetAsync(VersionedUrl, TestContext.Current.CancellationToken);
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnBadRequest_WhenCalledWithConflictingVersions()
        {
            const HttpStatusCode expectedStatusCode = HttpStatusCode.BadRequest;
            
            await using var factory = new ApiFactory();
            using var client = factory.CreateClient();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, VersionedUrl)
            {
                Headers =
                {
                    { Constants.ApiVersionHeaderKey, "1.1" }
                }
            };
            
            var response = await client.SendAsync(requestMessage, TestContext.Current.CancellationToken);
            Assert.Equal(expectedStatusCode, response.StatusCode);
        }
    }
}