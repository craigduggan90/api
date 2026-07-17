using Sol.Api.Client.V1.RequestModels;
using Sol.Api.Client.V1.ResponseModels;
using Sol.Core.Services.Jobs.Requests;
using Sol.Core.Services.Jobs.Responses;

namespace Sol.Api.Controllers.V1.Jobs;

public static class JobsMapper
{
    public static JobResponseModel ToJobResponseModel(JobModel model)
        => new(
            model.Id,
            model.Status,
            model.ErrorCode is not null 
                ? new JobErrorResponseModel(model.ErrorCode, model.ErrorMessage ?? "An unexpected error occurred.") 
                : null);
    
    public static GetJobsRequest ToGetJobsRequestRequest(this GetJobsRequestModel model) =>
        new(model.Type,
            model.Status,
            model.ErrorCode,
            model.LastEventFrom,
            model.LastEventTo,
            model.CreatedFrom,
            model.CreatedTo,
            model.ModifiedFrom,
            model.ModifiedTo,
            model.Cursor,
            model.PageSize);
}