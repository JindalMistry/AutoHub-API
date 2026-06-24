using AutoHub.Application.DTOs.Vehicles;

namespace AutoHub.Application.Interfaces;

public interface IVehicleImageService
{
    Task<List<VehicleImageResponse>> UploadImagesAsync(CreateVehicleImageRequest request);

    Task<List<VehicleImageResponse>> GetImagesAsync(
        Guid vehicleId);

    Task DeleteImageAsync(
        Guid imageId,
        Guid userId);
}