using AutoHub.Domain.Enums;

namespace AutoHub.Domain.Entities;

public class Inquiry
{
    public Guid Id { get; set; }

    public Guid? VehicleId { get; set; }

    public Guid BuyerId { get; set; }

    public Guid DealerId { get; set; }

    public string Message { get; set; } = string.Empty;

    public string DealerMessage {  get; set; } = string.Empty;

    public InquiryStatus Status { get; set; }

    public InquiryType Type { get; set; }

    public DateTime CreatedAt { get; set; }

    public Vehicle Vehicle { get; set; } = null!;

    public User Buyer { get; set; } = null!;

    public Dealer Dealer { get; set; } = null!;
}