namespace AutoHub.Application.DTOs.Vehicles;

public class VehicleImageResponse
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
