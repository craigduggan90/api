using Sol.Common.Extensions;
using Sol.Domain.Entities.Abstract;
using Sol.Domain.Enums;

namespace Sol.Domain.Entities;

public class Job(JobTypeEnum type, string? parameters) : EntityBase
{
    public JobTypeEnum Type { get; init; } = type;

    public JobStatusEnum Status { get; private set; } = JobStatusEnum.Pending;
    
    public DateTime? LastEventTime { get; private set; }
    
    public string? Parameters { get; init; } = parameters;
    
    public string? ErrorCode { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public override object AsSerializable()
        => new { Id, Type, Status, ErrorCode, DateCreated, DateModified, LastEventTime };

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