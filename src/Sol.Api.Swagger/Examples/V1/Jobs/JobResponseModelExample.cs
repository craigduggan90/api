using Sol.Api.Contracts.V1.ResponseModels;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger.Examples.V1.Jobs;

public class JobResponseModelExample : IExamplesProvider<JobResponseModel>
{
    public JobResponseModel GetExamples() => JobResponseModelFactory.GetExample();
}