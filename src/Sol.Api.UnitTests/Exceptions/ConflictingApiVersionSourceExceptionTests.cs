using Sol.Api.Exceptions;

namespace Sol.Api.UnitTests.Exceptions;

public static class ConflictingApiVersionSourceExceptionTests
{
    public class Constructor
    {
        [Fact]
        public void IncludesHeaderName_InMessage()
        {
            const string headerName = "X-Api-Version";

            var exception = new ConflictingApiVersionSourceException(headerName);

            Assert.Contains(headerName, exception.Message);
        }
    }
}