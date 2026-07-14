namespace Sol.Api.Controllers.V1.Jobs.ResponseModels;

public record JobErrorResponseModel(
    string Code, 
    string Message);