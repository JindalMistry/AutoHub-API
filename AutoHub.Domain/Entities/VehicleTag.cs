namespace AutoHub.Domain.Entities;

public class VehicleTag
{
    public Guid VehicleId { get; set; }

    public Guid TagId { get; set; }

    public Vehicle Vehicle { get; set; } = null!;

    public Tag Tag { get; set; } = null!;
}