using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [AllowAnonymous]
    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingVehiclesAsync()
    {
        var response = await _analyticsService.GetTrendingVehiclesAsync();

        return Ok(new ApiResponse<List<VehicleListingResponse>>
        {
            Success = true,
            Message = "Trending vehicles retrieved!",
            Data = response
        });
    }    
}
