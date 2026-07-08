using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using AutoHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [Authorize(Roles = "Dealer")]
    [HttpPost("add")]
    public async Task<IActionResult> Create(CreateVehicleRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var vehicle = await _vehicleService.CreateVehicleAsync(request, userId);

        return Ok(new ApiResponse<VehicleResponse>
        {
            Success = true,
            Message = "Vehicle created successfully",
            Data = vehicle
        });
    }

    [Authorize(Roles = "Dealer")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyVehicles()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var vehicles = await _vehicleService
            .GetMyVehiclesAsync(userId);

        return Ok(new ApiResponse<List<VehicleResponse>>
        {
            Success = true,
            Message = "Vehicles fetched successfully",
            Data = vehicles
        });
    }

    [AllowAnonymous]
    [HttpGet("{vehicleId}")]
    public async Task<IActionResult> GetVehicleById(Guid vehicleId)
    {
        var userId = Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var parsedUserId)
                    ? parsedUserId
                    : (Guid?)null;

        var vehicle = await _vehicleService
            .GetVehicleByIdAsync(vehicleId, userId);

        return Ok(new ApiResponse<VehicleResponse>
        {
            Success = true,
            Message = "Vehicle fetched successfully",
            Data = vehicle
        });
    }

    [Authorize(Roles = "Dealer, Admin")]
    [HttpGet("{vehicleId}/dealer")]
    public async Task<IActionResult> GetAnyVehicleAsync(Guid vehicleId)
    {
        var vehicle = await _vehicleService
            .GetAnyVehicleAsync(vehicleId);

        return Ok(new ApiResponse<VehicleResponse>
        {
            Success = true,
            Message = "Vehicle fetched successfully for high autohrity",
            Data = vehicle
        });
    }

    [Authorize(Roles = "Dealer")]
    [HttpPut("{vehicleId}")]
    public async Task<IActionResult> UpdateVehicle(Guid vehicleId, CreateVehicleRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var vehicle = await _vehicleService
            .UpdateVehicleAsync(
                request,
                userId,
                vehicleId);

        return Ok(new ApiResponse<VehicleResponse>
        {
            Success = true,
            Message = "Vehicle updated successfully",
            Data = vehicle
        });
    }

    [Authorize(Roles = "Dealer")]
    [HttpDelete("{vehicleId}")]
    public async Task<IActionResult> DeleteVehicle(Guid vehicleId)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        await _vehicleService
            .DeleteVehicleAsync(
                vehicleId,
                userId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Vehicle deleted successfully"
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{vehicleId}/publish")]
    public async Task<IActionResult> PublishVehicle(Guid vehicleId)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        await _vehicleService
            .PublishVehicleAsync(vehicleId, userId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Vehicle published successfully"
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{vehicleId}/unpublish")]
    public async Task<IActionResult> UnpublishVehicle(Guid vehicleId)
    {
        var adminId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        await _vehicleService
            .UnpublishVehicleAsync(
                vehicleId,
                adminId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Vehicle moved to draft successfully"
        });
    }

    [EnableRateLimiting("search")]
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> SearchVehicles([FromQuery] VehicleSearchRequest request)
    {
        var response = await _vehicleService.SearchVehiclesAsync(request);

        return Ok(new ApiResponse<PaginatedResponse<VehicleListingResponse>>
        {
            Success = true,
            Message = "Vehicles retrieved successfully.",
            Data = response
        });
    }

    [AllowAnonymous]
    [HttpGet("filter-options")]
    public async Task<IActionResult> GetFilterOptions()
    {
        var response =
            await _vehicleService.GetVehicleFilterOptionsAsync();

        return Ok(
            new ApiResponse<
                VehicleFilterOptionsResponse>
            {
                Success = true,
                Message =
                    "Filter options retrieved successfully.",
                Data = response
            });
    }
}
