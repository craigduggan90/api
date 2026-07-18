using Sol.Api.Contracts.V1.ResponseModels;
using Sol.Domain.Enums;

namespace Sol.Api.Swagger.Examples.V1.Jobs;

internal static class JobResponseModelFactory
{
    public static JobResponseModel GetExample()
        => new(
            "e4bf49d7100747ec9d3f4cbcb912d765", 
            nameof(JobStatusEnum.Pending));
}