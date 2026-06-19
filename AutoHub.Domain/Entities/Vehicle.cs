using AutoHub.Domain.Enums;

namespace AutoHub.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; }

    public Guid DealerId { get; set; }

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

    public VehicleStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    //Relationships
    public Dealer Dealer { get; set; } = null!;

    public ICollection<VehicleImage> Images { get; set; } = [];

    public ICollection<Favourite> Favorites { get; set; } = [];

    public ICollection<VehicleTag> VehicleTags { get; set; } = [];

    public VehicleAnalytics? Analytics { get; set; }

    public ICollection<Inquiry> Inquiries { get; set; }
    = [];
}