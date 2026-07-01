using AutoHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Inquiry;

public class InquirySearchRequest
{
    public InquiryStatus? Status { get; set; }

    public InquiryType? InquiryType { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
