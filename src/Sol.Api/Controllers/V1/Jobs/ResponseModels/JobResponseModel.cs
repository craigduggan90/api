namespace Sol.Api.Controllers.V1.Jobs.ResponseModels;

public record JobResponseModel(
    long Id, 
    string Status, 
    JobErrorResponseModel? Error = null);