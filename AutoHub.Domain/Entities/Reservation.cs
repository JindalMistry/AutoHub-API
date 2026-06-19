namespace AutoHub.Domain.Entities;

public class Reservation
{
    public Guid Id { get; set; }

    public Guid VehicleId { get; set; }

    public Guid UserId { get; set; }

    public ReservationStatus Status { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Vehicle Vehicle { get; set; } = null!;

    public User User { get; set; } = null!;
}