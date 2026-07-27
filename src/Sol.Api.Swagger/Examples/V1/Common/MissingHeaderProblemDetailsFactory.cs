using Microsoft.AspNetCore.Mvc;

namespace Sol.Api.Swagger.Examples.V1.Common;

public static class MissingHeaderProblemDetailsFactory
{
    public static ProblemDetails GetProblemDetails(string headerName) =>
        new()
        {
            Title = "Precondition Required",
            Status = 428,
            Type = "https://www.sol.api/errors/missing-header",
            Detail = $"'{headerName}' header value is required."
        };
}