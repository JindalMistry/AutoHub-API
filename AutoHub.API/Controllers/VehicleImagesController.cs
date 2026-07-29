using AutoHub.Application.Common;
using AutoHub.Application.DTOs;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/vehicle-images")]
public class VehicleImagesController : ControllerBase
{
    private readonly IVehicleImageService _vehicleImageService;

    public VehicleImagesController(
        IVehicleImageService vehicleImageService)
    {
        _vehicleImageService = vehicleImageService;
    }

    [Authorize(Roles = "Dealer")]
    [HttpPost("{vehicleId}/images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImages(
        Guid vehicleId,
        [FromForm] UploadVehicleImagesRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var input = new CreateVehicleImageRequest
        {
            VehicleId = vehicleId,
            UserId = userId,
            Files = request.Files
        };

        var response =
            await _vehicleImageService.UploadImagesAsync(input);

        return Ok(new ApiResponse<List<VehicleImageResponse>>
        {
            Success = true,
            Message = "Images uploaded successfully",
            Data = response
        });
    }

    [HttpGet("{vehicleId}/images")]
    public async Task<IActionResult> GetImages(Guid vehicleId)
    {
        var response = await _vehicleImageService
                .GetImagesAsync(vehicleId);

        return Ok(new ApiResponse<List<VehicleImageResponse>>
        {
            Success = true,
            Message = "Images retrieved successfully",
            Data = response
        });
    }

    [Authorize(Roles = "Dealer")]
    [HttpDelete("{imageId}")]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        await _vehicleImageService.DeleteImageAsync(imageId, userId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Images deleted successfully",
        });
    }

}