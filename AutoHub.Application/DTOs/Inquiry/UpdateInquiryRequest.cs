using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Inquiry;

public class UpdateInquiryRequest
{
    public InquiryStatus Status { get; set; }

    public string DealerMessage { get; set; } = string.Empty;
}