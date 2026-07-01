using AutoHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Inquiry;

public class InquiryResponse
{
    public Guid Id { get; set; }

    public Guid? VehicleId { get; set; }

    public string VehicleTitle { get; set; } = string.Empty;

    public string InquiryType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? DealerMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public string BuyerName { get; set; } = string.Empty;
}
