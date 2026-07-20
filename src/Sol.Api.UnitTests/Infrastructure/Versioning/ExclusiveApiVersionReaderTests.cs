using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Sol.Api.Exceptions;
using Sol.Api.Infrastructure.Versioning;
using Sol.Common;

namespace Sol.Api.UnitTests.Infrastructure.Versioning;

public static class ExclusiveApiVersionReaderTests
{
    public class Read
    {
        private static ExclusiveApiVersionReader CreateSut() => new();

        private static DefaultHttpContext CreateContext(IReadOnlyList<string>? routeVersions = null, string? headerVersion = null)
        {
            var context = new DefaultHttpContext();

            // Mirrors what ApiVersionRouteConstraint sets directly on the feature during
            // route matching — UrlSegmentApiVersionReader.Read() reads this, not RouteValues.
            context.Features.Set<IApiVersioningFeature>(new ApiVersioningFeature(context)
            {
                RawRequestedApiVersions = routeVersions ?? []
            });

            if (headerVersion is not null)
                context.Request.Headers[Constants.ApiVersionHeaderKey] = headerVersion;

            return context;
        }

        [Fact]
        public void ReturnsRouteValue_WhenOnlyRouteValueProvided()
        {
            var context = CreateContext(routeVersions: ["1.0"]);

            var sut = CreateSut();
            var actual = sut.Read(context.Request);

            Assert.Equal(["1.0"], actual);
        }

        [Fact]
        public void ReturnsHeaderValue_WhenOnlyHeaderProvided()
        {
            var context = CreateContext(headerVersion: "1.0");

            var sut = CreateSut();
            var actual = sut.Read(context.Request);

            Assert.Equal(["1.0"], actual);
        }

        [Fact]
        public void ReturnsEmpty_WhenNeitherProvided()
        {
            var context = CreateContext();

            var sut = CreateSut();
            var actual = sut.Read(context.Request);

            Assert.Empty(actual);
        }

        [Fact]
        public void Throws_WhenBothProvided_AndValuesMatch()
        {
            var context = CreateContext(routeVersions: ["1.0"], headerVersion: "1.0");

            var sut = CreateSut();

            Assert.Throws<ConflictingApiVersionSourceException>(() => sut.Read(context.Request));
        }

        [Fact]
        public void Throws_WhenBothProvided_AndValuesDiffer()
        {
            var context = CreateContext(routeVersions: ["1.0"], headerVersion: "2.0");

            var sut = CreateSut();

            Assert.Throws<ConflictingApiVersionSourceException>(() => sut.Read(context.Request));
        }
    }

    public class AddParameters
    {
        private static ExclusiveApiVersionReader CreateSut() => new();

        [Fact]
        public void DoesNotThrow_WhenDelegatingToUnderlyingReaders()
        {
            var context = Substitute.For<IApiVersionParameterDescriptionContext>();

            var sut = CreateSut();
            var exception = Record.Exception(() => sut.AddParameters(context));

            Assert.Null(exception);
        }
    }
}