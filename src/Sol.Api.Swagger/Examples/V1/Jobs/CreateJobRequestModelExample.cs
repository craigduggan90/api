using Sol.Api.Contracts.V1.RequestModels;
using Sol.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;

namespace Sol.Api.Swagger.Examples.V1.Jobs;

public class CreateJobRequestModelExample : IExamplesProvider<CreateJobRequestModel>
{
    public CreateJobRequestModel GetExamples() =>
        new CreateJobRequestModel(
            nameof(JobTypeEnum.ArchiveProjectJob),
            JsonSerializer.SerializeToElement(new
            {
                Property = "value", Nested = new { OtherProperty = "otherValue" }
            }));
}