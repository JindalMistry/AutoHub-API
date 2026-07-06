using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/cache")]
public class CacheController : ControllerBase
{
    private readonly ICacheService _cacheService;

    public CacheController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    [HttpGet("cache-test")]
    public async Task<IActionResult> Test()
    {
        await _cacheService.SetAsync(
            "test-key",
            "hello redis",
            TimeSpan.FromMinutes(5));

        var value =
            await _cacheService
                .GetAsync<string>("test-key");

        return Ok(value);
    }
}
