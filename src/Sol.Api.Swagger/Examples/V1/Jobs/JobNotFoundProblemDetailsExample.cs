using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger.Examples.V1.Jobs;

public class JobNotFoundProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples() =>
        new()
        {
            Status = 404,
            Detail = "Unable to find Job with Id: '369e1248fa4b418baef47ae937462d34'.",
            Type = "https://www.sol.api/errors/not-found",
            Title = "Resource Not Found",
            Extensions = new Dictionary<string, object?>
            {
                { "resource", "Job" },
                { "identifier", "369e1248fa4b418baef47ae937462d34" }
            }
        };
}