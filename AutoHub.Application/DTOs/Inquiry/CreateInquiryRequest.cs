using AutoHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Inquiry;

public class CreateInquiryRequest
{
    public Guid? DealerId { get; set; }

    public Guid? VehicleId { get; set; }

    public InquiryType InquiryType { get; set; }

    public string Message { get; set; } = string.Empty;
}
