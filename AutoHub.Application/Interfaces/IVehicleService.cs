using AutoHub.Application.DTOs.Vehicles;

namespace AutoHub.Application.Interfaces;

public interface IVehicleService
{
    Task<VehicleResponse> CreateVehicleAsync(CreateVehicleRequest request, Guid userId);

    Task<List<VehicleResponse>> GetMyVehiclesAsync(Guid userId);

    Task<VehicleResponse> GetVehicleByIdAsync(Guid vehicleId, Guid? userId);

    Task<VehicleResponse> GetAnyVehicleAsync(Guid vehicleId);

    Task<VehicleResponse> UpdateVehicleAsync(CreateVehicleRequest request, Guid userId, Guid vehicleId);

    Task PublishVehicleAsync(Guid vehicleId, Guid userId);

    Task DeleteVehicleAsync(Guid vehicleId, Guid userId);

    Task UnpublishVehicleAsync(Guid vehicleId, Guid adminId);

    Task<PaginatedResponse<VehicleListingResponse>> SearchVehiclesAsync(VehicleSearchRequest request);

    Task<VehicleFilterOptionsResponse> GetVehicleFilterOptionsAsync();
}
