using Sol.Api.Contracts.V1.ResponseModels;
using Sol.Common.Pagination;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Swagger.Examples.V1.Jobs;

public class JobResponseModelPageExample : IExamplesProvider<PagedList<JobResponseModel>>
{
    public PagedList<JobResponseModel> GetExamples()
        => new([JobResponseModelFactory.GetExample()], "MTc4NDM3ODUzODQ5ODIxMw==", 1);
}