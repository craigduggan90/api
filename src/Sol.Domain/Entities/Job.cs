using Sol.Common.Extensions;
using Sol.Domain.Entities.Abstract;
using Sol.Domain.Enums;

namespace Sol.Domain.Entities;

public class Job : EntityBase
{
    public Job(string idempotencyKey, JobTypeEnum type, string? parameters)
    {
        IdempotencyKey = idempotencyKey;
        Type = type;
        Parameters = parameters;
        LastEventTime = DateCreated;
    }

    public string IdempotencyKey { get; init; }
    
    public JobTypeEnum Type { get; init; }

    public JobStatusEnum Status { get; private set; } = JobStatusEnum.Pending;

    public DateTime LastEventTime { get; private set; }
    
    public string? Parameters { get; init; }
    
    public string? ErrorCode { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public override object AsSerializable()
        => new { Id, IdempotencyKey, Type, Status, ErrorCode, DateCreated, DateModified, LastEventTime };

    public override string ToString() => AsSerializable().Serialize();

    public void Update(
        DateTime eventTime,
        JobStatusEnum status, 
        string? errorCode, 
        string? errorMessage)
    {
        if (LastEventTime >= eventTime)
            return;
        
        UpdateProperty(nameof(Status), status);
        UpdateProperty(nameof(ErrorCode), errorCode);
        UpdateProperty(nameof(ErrorMessage), errorMessage);
        UpdateProperty(nameof(LastEventTime), eventTime);
    }

    public void Delete()
    {
        if (DateDeleted.HasValue)
            return;
        
        SetDateModified();
        SoftDelete();
    }
}