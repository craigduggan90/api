using Microsoft.AspNetCore.Mvc;

namespace Sol.Api.Controllers.V1.Jobs;

[ApiController]
[Route("api/[controller]")]
public class JobsController() : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetJobs(
        [FromQuery] object request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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