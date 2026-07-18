using Microsoft.AspNetCore.Mvc;
using Sol.Api.Contracts.V1.RequestModels;
using Sol.Api.Contracts.V1.ResponseModels;
using Sol.Api.Swagger.Examples.V1.Jobs;
using Sol.Common.Extensions;
using Sol.Core.Services.Jobs;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Sol.Api.Controllers.V1.Jobs;

[ApiController]
[Route("api/[controller]")]
public class JobsController(IJobsService jobsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetJobs(
        [FromQuery] GetJobsRequestModel query,
        CancellationToken cancellationToken)
    {
        var result = await jobsService.GetJobsAsync(query.ToGetJobsRequestRequest(), cancellationToken);
        return Ok(result.Map(JobsMapper.ToJobResponseModel));
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType<JobResponseModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(JobResponseModelExample))]
    public async Task<IActionResult> GetJobById(
        string id,
        CancellationToken cancellationToken)
    {
        var job = await jobsService.GetJobByIdAsync(id, cancellationToken);
        return Ok(job.ToJobResponseModel());
    }

    [HttpPost]
    [SwaggerRequestExample(typeof(CreateJobRequestModel), typeof(CreateJobRequestModelExample))]
    [ProducesResponseType<JobResponseModel>(202)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerResponseExample(202, typeof(JobResponseModelExample))]
    public async Task<IActionResult> CreateJob(
        [FromBody] CreateJobRequestModel request,
        [FromHeader] string idempotency,
        CancellationToken cancellationToken)
    {
        var model = request.ToCreateJobRequest(idempotency);
        var job = await jobsService.CreateJobAsync(model, cancellationToken);
        return AcceptedAtAction(nameof(GetJobById), new { id = job.Id }, job.ToJobResponseModel());
    }
        
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJob(
        [FromRoute] long id,
        [FromBody] object request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}