namespace AutoHub.Domain.Entities;

public class Favourite
{
    public Guid UserId { get; set; }

    public Guid VehicleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;

    public Vehicle Vehicle { get; set; } = null!;
}