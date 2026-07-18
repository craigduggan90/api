namespace Sol.Core.Services.Jobs.Requests;

public record UpdateJobRequest(
    string Id,
    string Status,
    string? ErrorCode,
    string? ErrorMessage);