using Microsoft.AspNetCore.Mvc;
using Sol.Api.Contracts.V1.RequestModels;
using Sol.Common.Extensions;
using Sol.Core.Services.Jobs;

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
    public async Task<IActionResult> GetJobById(
        long id,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> CreateJob(
        [FromBody] object request,
        [FromHeader] string idempotency,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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