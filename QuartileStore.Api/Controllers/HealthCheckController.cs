using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuartileStore.Api.Controllers;

[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet("/health")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Healthy()
    {
        return Ok( new { Status = "Healthy" });
    }
}