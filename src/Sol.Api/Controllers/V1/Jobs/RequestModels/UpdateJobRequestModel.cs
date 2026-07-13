namespace Sol.Api.Controllers.V1.Jobs.RequestModels;

public record UpdateJobRequestModel(
    string EventId,
    string Status,
    DateTimeOffset EventTime,
    string? ErrorCode,
    string? ErrorMessage);
