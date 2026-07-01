using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Favourite;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using AutoHub.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/favourites")]
public class FavouriteController : ControllerBase
{
    private readonly IFavouriteService _favouriteService;

    public FavouriteController(IFavouriteService favouriteService)
    {
        _favouriteService = favouriteService;
    }

    [Authorize(Roles = "Buyer")]
    [HttpPost]
    public async Task<IActionResult> AddFavourite(AddFavouriteRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _favouriteService
            .AddFavouriteAsync(request.VehicleId, userId);

        return Ok(
            new ApiResponse<object>
            {
                Success = true,
                Message =
                    "Vehicle added to favourites."
            });
    }

    [Authorize(Roles = "Buyer")]
    [HttpDelete]
    public async Task<IActionResult> RemoveFavourite(RemoveFavouriteRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _favouriteService
            .RemoveFavouriteAsync(request.VehicleId,userId);

        return Ok(
            new ApiResponse<object>
            {
                Success = true,
                Message =
                    "Vehicle removed from favourites."
            });
    }

    [Authorize(Roles = "Buyer")]
    [HttpGet]
    public async Task<IActionResult> GetMyFavourites(int pageNumber = 1, int pageSize = 10)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var response =
            await _favouriteService
                .GetMyFavouritesAsync(
                    userId,
                    pageNumber,
                    pageSize);

        return Ok(
            new ApiResponse<PaginatedResponse<VehicleListingResponse>>
            {
                Success = true,
                Message =
                    "Favourites retrieved successfully.",
                Data = response
            });
    }
}
