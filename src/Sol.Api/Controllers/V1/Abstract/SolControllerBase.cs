using Microsoft.AspNetCore.Mvc;

namespace Sol.Api.Controllers.V1.Abstract;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class SolControllerBase : ControllerBase;