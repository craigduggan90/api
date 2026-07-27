using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger.Examples.V1.Common;

public class CommandValidationProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples() =>
        new()
        {
            Status = 422,
            Detail = "One or more validation failures occurred.",
            Type = "https://www.sol.api/errors/validation",
            Title = "Validation Error",
            Extensions = new Dictionary<string, object?>
            {
                {
                    "errors",
                    new Dictionary<string, string[]>
                    {
                        {
                            "fieldName", [ "first validation error", "second validation error" ]
                        }
                    }
                }
            }
        };
}