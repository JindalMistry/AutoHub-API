using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class BackgroundJobController : ControllerBase
{
    private readonly IBackgroundJobService _backgroundJobService;
    public BackgroundJobController(IBackgroundJobService backgroundJobService) 
    {
        _backgroundJobService = backgroundJobService;
    }

    [HttpPost("recalculate-trending")]
    public async Task<IActionResult> RecalculateTrending()
    {
        await _backgroundJobService.RecalculateTrendingScoresAsync();
        return Ok();
    }

    [HttpPost("expire-reservations")]
    public async Task<IActionResult> ExpireReservations()
    {
        await _backgroundJobService.ExpireReservationsAsync();
        return Ok();
    }
}
