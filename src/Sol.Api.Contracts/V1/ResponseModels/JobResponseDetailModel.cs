namespace Sol.Api.Contracts.V1.ResponseModels;

public record JobResponseDetailModel(
    string Id,
    string IdempotencyKey,
    string ConcurrencyToken,
    string Type,
    string Status,
    object? Parameters,
    DateTime DateCreated,
    DateTime DateLastModified,
    JobResponseErrorModel? Error = null)
    : JobResponseModel(Id, Status, IdempotencyKey, ConcurrencyToken, Error);