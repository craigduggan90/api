namespace Sol.Api.Contracts.V1.ResponseModels;

public record JobResponseModel(string Id, string Status, JobErrorResponseModel? Error = null);