using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger.Examples.V1.Common;

public class MissingIdempotencyKeyExample : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples()
        => MissingHeaderProblemDetailsFactory.GetProblemDetails(SolConstants.IdempotencyHeaderKey);
}