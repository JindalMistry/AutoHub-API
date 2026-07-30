using Microsoft.AspNetCore.Mvc;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeploymentTestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Message = "🚀 Auto deployment is working!",
            DeployedAt = DateTime.UtcNow,
            Machine = Environment.MachineName,
            Version = "Release 1.0"
        });
    }
}