using AutoHub.Application.DTOs.Inquiry;
using AutoHub.Application.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.Interfaces;

public interface IInquiryService
{
    Task CreateInquiryAsync(
        CreateInquiryRequest request,
        Guid buyerId);

    Task<PaginatedResponse<InquiryResponse>>
        GetMyInquiriesAsync(
            Guid buyerId,
            InquirySearchRequest request);

    Task<InquiryResponse>
        GetInquiryByIdAsync(
            Guid inquiryId,
            Guid userId);

    Task<PaginatedResponse<InquiryResponse>>
        GetDealerInquiriesAsync(
            Guid userId,
            InquirySearchRequest request);

    Task UpdateInquiryAsync(
        Guid inquiryId,
        UpdateInquiryRequest request,
        Guid userId);
}
