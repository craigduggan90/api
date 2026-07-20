using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sol.Api.Controllers.V1;

namespace Sol.Api.UnitTests.Controllers.V1;

public static class HealthControllerTests
{
    public class Ping
    {
        private static HealthController GetOrCreateSut() =>
            new()
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

        [Fact]
        public void ShouldReturnNoContent()
        {
            var sut = GetOrCreateSut();
            var result = sut.Ping();

            Assert.IsType<NoContentResult>(result);
        }
    }
}