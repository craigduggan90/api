using Asp.Versioning;
using Sol.Api.Exceptions;
using Sol.Common;

namespace Sol.Api.Infrastructure.Versioning;

public class ExclusiveApiVersionReader : IApiVersionReader
{
    private static readonly UrlSegmentApiVersionReader RouteReader = new();
    private static readonly HeaderApiVersionReader HeaderReader = new() { HeaderNames = { Constants.ApiVersionHeaderKey } };

    public IReadOnlyList<string> Read(HttpRequest request)
    {
        var routeValues = RouteReader.Read(request);
        var headerValues = HeaderReader.Read(request);

        if (routeValues.Count > 0 && headerValues.Count > 0)
            throw new ConflictingApiVersionSourceException(Constants.ApiVersionHeaderKey);

        return routeValues.Count > 0 ? routeValues : headerValues;
    }

    public void AddParameters(IApiVersionParameterDescriptionContext context)
    {
        RouteReader.AddParameters(context);
        HeaderReader.AddParameters(context);
    }
}