using System.Text.Json;

namespace Sol.Api.Client.V1.RequestModels;

public record CreateJobRequestModel(string Type, JsonElement? Parameters);