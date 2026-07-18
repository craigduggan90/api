using Microsoft.AspNetCore.Mvc;
using Sol.Api.Attributes;
using Sol.Api.Contracts.V1.RequestModels;
using Sol.Api.Contracts.V1.ResponseModels;
using Sol.Api.Controllers.V1.Abstract;
using Sol.Api.Infrastructure;
using Sol.Api.Swagger.Examples.V1.Common;
using Sol.Api.Swagger.Examples.V1.Jobs;
using Sol.Common.Extensions;
using Sol.Common.Pagination;
using Sol.Core.Services.Jobs;
using Swashbuckle.AspNetCore.Filters;

namespace Sol.Api.Controllers.V1.Jobs;

public class JobsController(IJobsService jobsService) : SolControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedList<JobResponseModel>>(200)]
    [ProducesResponseType<ProblemDetails>(400)]
    [SwaggerResponseExample(200, typeof(JobResponseModelPageExample))]
    [SwaggerResponseExample(400, typeof(QueryValidationProblemDetailsExample))]
    public async Task<IActionResult> GetJobs(
        [FromQuery] GetJobsRequestModel query,
        CancellationToken cancellationToken)
    {
        var result = await jobsService.GetJobsAsync(query.ToGetJobsRequestRequest(), cancellationToken);
        return Ok(result.Map(JobsMapper.ToJobResponseModel));
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType<JobResponseDetailModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(JobResponseDetailModelExample))]
    [SwaggerResponseExample(404, typeof(JobNotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetJobById(
        string id,
        CancellationToken cancellationToken)
    {
        var job = await jobsService.GetJobByIdAsync(id, cancellationToken);
        return Ok(job.ToJobResponseDetailModel());
    }

    [HttpPost]
    [RequiresHeader(Constants.IdempotencyHeaderKey)]
    [SwaggerRequestExample(typeof(CreateJobRequestModel), typeof(CreateJobRequestModelExample))]
    [ProducesResponseType<JobResponseModel>(202)]
    [ProducesResponseType<ProblemDetails>(422)]
    [ProducesResponseType<ProblemDetails>(428)]
    [SwaggerResponseExample(202, typeof(JobResponseModelExample))]
    public async Task<IActionResult> CreateJob(
        [FromBody] CreateJobRequestModel request,
        [FromHeader(Name = Constants.IdempotencyHeaderKey)] string? idempotency,
        CancellationToken cancellationToken)
    {
        var model = request.ToCreateJobRequest(idempotency);
        var job = await jobsService.CreateJobAsync(model, cancellationToken);
        return AcceptedAtAction(nameof(GetJobById), new { id = job.Id }, job.ToJobResponseModel());
    }
        
    [HttpPut("{id}")]
    [RequiresHeader(Constants.IfMatchHeaderKey)] 
    [ProducesResponseType<JobResponseModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(412)]
    [ProducesResponseType<ProblemDetails>(428)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerResponseExample(200, typeof(JobResponseModelExample))]
    [SwaggerResponseExample(404, typeof(JobNotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(CommandValidationProblemDetailsExample))]
    public async Task<IActionResult> UpdateJob(
        [FromRoute] string id,
        [FromHeader(Name = Constants.IfMatchHeaderKey)] string? concurrencyToken,
        [FromBody] UpdateJobRequestModel request,
        CancellationToken cancellationToken)
    {
        var model = request.ToUpdateJobRequest(id, concurrencyToken);
        var job = await jobsService.UpdateJobAsync(model, cancellationToken);
        return Ok(job.ToJobResponseModel());
    }
}