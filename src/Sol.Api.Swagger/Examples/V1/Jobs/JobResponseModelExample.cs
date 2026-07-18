using Sol.Api.Contracts.V1.ResponseModels;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger.Examples.V1.Jobs;

public class JobResponseModelExample : IExamplesProvider<JobResponseModel>
{
    public JobResponseModel GetExamples()
        => new JobResponseModel("3aff52756b1944489b1a3f85bf8d3d91", "Pending");
}