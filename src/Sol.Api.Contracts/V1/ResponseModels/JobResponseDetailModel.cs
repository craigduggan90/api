namespace Sol.Api.Contracts.V1.ResponseModels;

public record JobResponseDetailModel(
    string Id,
    string IdempotencyKey,
    string Type,
    string Status, 
    object? Parameters,
    DateTime DateCreated,
    DateTime DateLastModified,
    JobErrorResponseModel? Error = null)
    : JobResponseModel(Id, Status, Error);