using AutoHub.Infrastructure.Persistance;
using Microsoft.AspNetCore.Mvc;

namespace AutoHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("healthy");
        }
    }
}
