using Microsoft.AspNetCore.Http;

namespace AutoHub.Application.DTOs.Vehicles;

public class CreateVehicleImageRequest
{
    public Guid VehicleId { get; set; }
    public Guid UserId { get; set; }
    public required List<IFormFile> Files { get; set; }
}
