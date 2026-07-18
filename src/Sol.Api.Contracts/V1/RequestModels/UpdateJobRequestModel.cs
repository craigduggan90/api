namespace Sol.Api.Contracts.V1.RequestModels;

public record UpdateJobRequestModel(
    string Status,
    string? ErrorCode,
    string? ErrorMessage);