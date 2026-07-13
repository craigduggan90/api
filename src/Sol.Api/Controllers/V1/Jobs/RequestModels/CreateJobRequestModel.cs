using System.Text.Json;

namespace Sol.Api.Controllers.V1.Jobs.RequestModels;

public record CreateJobRequestModel(
    string Type,
    JsonElement? Parameters)
{
    public bool HasParameters => 
        Parameters is not null && 
        Parameters.Value.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined);
}