using AutoHub.Domain.Enums;

namespace AutoHub.Domain.Entities;

public class Dealer
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string BusinessName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Pincode { get; set; } = string.Empty;

    public DealerStatus Status { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public Guid? ApprovedByUserId { get; set; }

    //Relationships
    public User User { get; set; } = null!;

    public ICollection<Vehicle> Vehicles { get; set; } = [];

    public ICollection<Inquiry> Inquiries { get; set; }
    = [];
}