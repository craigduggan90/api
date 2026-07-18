using Microsoft.AspNetCore.Mvc;
using Sol.Common;

namespace Sol.Api.Controllers.V1.Abstract;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class SolControllerBase : ControllerBase
{
    protected void SetEtagResponseHeader(string value)
        => SetResponseHeader(Constants.ETagHeaderKey, $"\"{value}\"");
    
    private void SetResponseHeader(string key, string value) 
        => Response.Headers.TryAdd(key, value);
}