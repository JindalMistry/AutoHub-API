using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Admin;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    public AdminController (IAdminService adminService)
    {
        _adminService = adminService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardAsync()
    {
        var response = await _adminService.GetDashboardAsync();

        return Ok(new ApiResponse<AdminDashboardResponse>
        {
            Success = true,
            Message = "Dashboard data retrieved!",
            Data = response
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("top-vehicles")]
    public async Task<IActionResult> GetTopVehiclesAsync()
    {
        var response = await _adminService.GetTopVehiclesAsync();

        return Ok(new ApiResponse<List<TopVehiclesResponse>>
        {
            Success = true,
            Message = "Top vehicles data retrieved!",
            Data = response
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("top-dealers")]
    public async Task<IActionResult> GetTopDealersAsync()
    {
        var response = await _adminService.GetTopDealersAsync();

        return Ok(new ApiResponse<List<TopDealerResponse>>
        {
            Success = true,
            Message = "Top dealers data retrieved!",
            Data = response
        });
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("pending-vehicles")]
    public async Task<IActionResult> GetPendingVehiclesAsync()
    {
        var response = await _adminService.GetPendingVehiclesAsync();

        return Ok(new ApiResponse<List<VehicleListingResponse>>
        {
            Success = true,
            Message = "Vehicle with status Draft retrieved!",
            Data = response
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("pending-dealers")]
    public async Task<IActionResult> GetPendingDealersAsync()
    {
        var response = await _adminService.GetPendingDealerAsync();

        return Ok(new ApiResponse<List<PendingDealerResponse>>
        {
            Success = true,
            Message = "Pending dealers retrieved!",
            Data = response
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("completed-reservations")]
    public async Task<IActionResult> GetCompletedReservationsAsync()
    {
        var response = await _adminService.GetActiveReservationsAsync();

        return Ok(new ApiResponse<List<CompletedReservationResponse>>
        {
            Success = true,
            Message = "Completed reservations retrieved!",
            Data = response
        });
    }

}