using AutoHub.Application.DTOs.Vehicles;

namespace AutoHub.Application.Interfaces;

public interface IFavouriteService
{
    Task AddFavouriteAsync(
        Guid vehicleId,
        Guid userId);

    Task RemoveFavouriteAsync(
        Guid vehicleId,
        Guid userId);

    Task<PaginatedResponse<VehicleListingResponse>>
        GetMyFavouritesAsync(
            Guid userId,
            int pageNumber,
            int pageSize);
}
