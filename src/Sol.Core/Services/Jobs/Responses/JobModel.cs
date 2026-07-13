using Sol.Domain.Entities;

namespace Sol.Core.Services.Jobs.Responses;

public record JobModel(
    string Id,
    string IdempotencyKey,
    long Cursor,
    string Type,
    string Status,
    string? ErrorCode,
    string? ErrorMessage,
    DateTime DateCreated,
    DateTime DateModified,
    DateTime LastEventTime)
{
    public static JobModel FromEntity(Job job)
        => new(
            Id: job.Id,
            IdempotencyKey: job.IdempotencyKey,
            Cursor: job.Cursor,
            Type: job.Type.ToString(),
            Status: job.Status.ToString(),
            ErrorCode: job.ErrorCode,
            ErrorMessage: job.ErrorMessage,
            DateCreated: job.DateCreated,
            DateModified: job.DateModified,
            LastEventTime: job.LastEventTime);
};