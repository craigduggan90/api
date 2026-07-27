using System.Text.Json;

namespace Sol.Api.Contracts.V1.RequestModels;

public record CreateJobRequestModel(string Type, JsonElement? Parameters);