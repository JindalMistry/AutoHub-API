using AutoHub.Domain.Enums;

namespace AutoHub.Application.DTOs.Vehicles;

public class CreateVehicleRequest
{
    public string Title { get; set; } = string.Empty;

    public string RegNo { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string Variant { get; set; } = string.Empty;

    public int Year { get; set; }

    public int Mileage { get; set; }

    public FuelType FuelType { get; set; }

    public TransmissionType Transmission { get; set; }

    public string Description { get; set; } = string.Empty;
}
