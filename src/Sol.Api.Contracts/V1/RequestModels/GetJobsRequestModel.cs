namespace Sol.Api.Contracts.V1.RequestModels;

public record GetJobsRequestModel(
    string? Type = null,
    string? Status = null,
    string? ErrorCode = null,
    DateTime? LastEventFrom = null,
    DateTime? LastEventTo = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    DateTime? ModifiedFrom = null,
    DateTime? ModifiedTo = null,
    string? Cursor = null,
    int? PageSize = null);