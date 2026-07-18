using Sol.Api.Contracts.V1.ResponseModels;
using Sol.Domain.Enums;

namespace Sol.Api.Swagger.Examples.V1.Jobs;

internal static class JobResponseModelFactory
{
    public static JobResponseModel GetExample()
        => new(
            "e4bf49d7100747ec9d3f4cbcb912d765",
            nameof(JobStatusEnum.Pending),
            "76b06dd6-416c-4a86-a97c-7316158f1a95",
            "ea6e3216715d930fe856dab86ac5afbe");
}