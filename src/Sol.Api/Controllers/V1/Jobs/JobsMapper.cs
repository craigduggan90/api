using Sol.Api.Contracts.V1.RequestModels;
using Sol.Api.Contracts.V1.ResponseModels;
using Sol.Common.Extensions;
using Sol.Core.Services.Jobs.Requests;
using Sol.Core.Services.Jobs.Responses;
using System.Text.Json;

namespace Sol.Api.Controllers.V1.Jobs;

public static class JobsMapper
{
    public static JobResponseModel ToJobResponseModel(this JobModel model) =>
        new(model.Id, model.Status, model.ToJobErrorResponseModel());
    
    public static JobResponseDetailModel ToJobResponseDetailModel(this JobModel model) => 
        new(
            model.Id,
            model.IdempotencyKey,
            model.Type,
            model.Status,
            model.Parameters?.Deserialize<JsonElement>(),
            model.DateCreated,
            model.DateModified,
            model.ToJobErrorResponseModel());

    public static CreateJobRequest ToCreateJobRequest(
        this CreateJobRequestModel model,
        string? idempotencyKey)
        => new(
            idempotencyKey ?? string.Empty,
            model.Type,
            model.Parameters is null || model.Parameters.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
                ? null
                : model.Parameters.Value.GetRawText());

    public static UpdateJobRequest ToUpdateJobRequest(
        this UpdateJobRequestModel model,
        string id) => new UpdateJobRequest(id, model.Status, model.ErrorCode, model.ErrorMessage);
    
    public static GetJobsRequest ToGetJobsRequestRequest(this GetJobsRequestModel model) =>
        new(model.Type,
            model.Status,
            model.ErrorCode,
            model.CreatedFrom,
            model.CreatedTo,
            model.ModifiedFrom,
            model.ModifiedTo,
            model.Cursor,
            model.PageSize);

    private static JobErrorResponseModel? ToJobErrorResponseModel(this JobModel model) =>
        model.ErrorCode is not null
            ? new JobErrorResponseModel(model.ErrorCode, model.ErrorMessage ?? "An unexpected error occurred.")
            : null;
}