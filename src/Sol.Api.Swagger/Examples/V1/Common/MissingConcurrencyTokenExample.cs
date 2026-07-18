using Microsoft.AspNetCore.Mvc;
using Sol.Common;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger.Examples.V1.Common;

public class MissingConcurrencyTokenExample : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples()
        => MissingHeaderProblemDetailsFactory.GetProblemDetails(Constants.IfMatchHeaderKey);
}
