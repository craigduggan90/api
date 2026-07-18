using Microsoft.AspNetCore.Mvc;
using Sol.Api.Swagger.Examples.V1.Common;
using Sol.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger.Examples.V1.Jobs;

public class JobNotFoundProblemDetailsFactory : IExamplesProvider<ProblemDetails>
{
    public ProblemDetails GetExamples() =>
        NotFoundProblemDetailsFactory.GetProblemDetails(nameof(Job), "9ab662b7adc545198696085b8c61ec93");

}