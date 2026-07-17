namespace Sol.Api.Contracts.V1.RequestModels;

public record UpdateJobRequestModel(
    string EventId,
    string Status,
    DateTimeOffset EventTime,
    string? ErrorCode,
    string? ErrorMessage);
