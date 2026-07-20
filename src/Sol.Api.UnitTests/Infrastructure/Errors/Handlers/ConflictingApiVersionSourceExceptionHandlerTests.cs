using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sol.Api.Exceptions;
using Sol.Api.Infrastructure.Errors.Handlers;

namespace Sol.Api.UnitTests.Infrastructure.Errors.Handlers;

public static class ConflictingApiVersionSourceExceptionHandlerTests
{
    public class TryHandleAsync
    {
        private static ConflictingApiVersionSourceExceptionHandler CreateSut() => new();

        [Fact]
        public async Task ReturnsFalse_WhenExceptionNotConflictingApiVersionSourceException()
        {
            var exception = new InvalidOperationException("that was an invalid operation!");

            var sut = CreateSut();
            var actual = await sut.TryHandleAsync(new DefaultHttpContext(), exception, CancellationToken.None);
            Assert.False(actual);
        }

        [Fact]
        public async Task ReturnsTrue_WhenConflictingApiVersionSourceException()
        {
            var exception = new ConflictingApiVersionSourceException("Api-Version");

            const int expectedStatus = ConflictingApiVersionSourceExceptionHandler.StatusCode;
            var expectedContent = ConflictingApiVersionSourceExceptionHandler.GetProblemDetails(exception);

            using var responseStream = new MemoryStream();
            var context = new DefaultHttpContext { Response = { Body = responseStream } };

            var sut = CreateSut();
            var actual = await sut.TryHandleAsync(context, exception, CancellationToken.None);
            Assert.True(actual);
            Assert.Equal(expectedStatus, context.Response.StatusCode);

            var actualContent = await responseStream.RewindAndReadAsync<ProblemDetails>();
            Assert.Equivalent(expectedContent, actualContent);
        }
    }
}