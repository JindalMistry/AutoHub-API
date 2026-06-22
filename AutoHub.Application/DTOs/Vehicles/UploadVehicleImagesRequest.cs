using Microsoft.AspNetCore.Http;

namespace AutoHub.Application.DTOs.Vehicles;

public class UploadVehicleImagesRequest
{
    public List<IFormFile> Files { get; set; } = [];
}
