using Microsoft.AspNetCore.Mvc;

namespace Sol.Api.Swagger.Examples.V1.Common;

public static class NotFoundProblemDetailsFactory
{
    public static ProblemDetails GetProblemDetails(string resource, string identifier) =>
        new()
        {
            Status = 404,
            Detail = $"Unable to find {resource} with Id: '{identifier}'.",
            Type = "https://www.sol.api/errors/not-found",
            Title = "Resource Not Found",
            Extensions = new Dictionary<string, object?>
            {
                { "resource", resource },
                { "identifier", identifier }
            }
        };
}