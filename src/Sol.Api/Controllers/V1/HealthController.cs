using Microsoft.AspNetCore.Mvc;
using Sol.Api.Controllers.V1.Abstract;

namespace Sol.Api.Controllers.V1;

public class HealthController : SolControllerBase
{
    [HttpGet, ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Ping()
        => NoContent();
}