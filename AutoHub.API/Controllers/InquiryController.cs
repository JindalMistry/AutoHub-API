using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Inquiry;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/inquiry")]
public class InquiryController : ControllerBase
{
    private readonly IInquiryService _inquiryService;

    public InquiryController(
        IInquiryService inquiryService)
    {
        _inquiryService = inquiryService;
    }

    [Authorize(Roles = "Buyer")]
    [HttpPost]
    public async Task<IActionResult> CreateInquiry(CreateInquiryRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _inquiryService.CreateInquiryAsync(request, userId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Inquiry created successfully!"
        });
    }

    [Authorize(Roles = "Buyer")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyInquiries([FromQuery] InquirySearchRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var response = await _inquiryService.GetMyInquiriesAsync(userId, request);

        return Ok(new ApiResponse<PaginatedResponse<InquiryResponse>>
        {
            Success = true,
            Message = "Your inquiries retrieved successfully!",
            Data = response
        });
    }

    [Authorize(Roles = "Buyer,Dealer")]
    [HttpGet("{inquiryId}")]
    public async Task<IActionResult> GetInquiry(Guid inquiryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var response = await _inquiryService.GetInquiryByIdAsync(inquiryId, userId);

        return Ok(new ApiResponse<InquiryResponse>
        {
            Success = true,
            Message = "Inquiry retrieved successfully!",
            Data = response
        });
    }

    [Authorize(Roles = "Dealer")]
    [HttpGet("dealer")]
    public async Task<IActionResult> GetDealerInquiries([FromQuery] InquirySearchRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var response = await _inquiryService.GetDealerInquiriesAsync(userId, request);

        return Ok(new ApiResponse<PaginatedResponse<InquiryResponse>>
        {
            Success = true,
            Message = "Your inquiries as a dealer retrieved.",
            Data = response
        });
    }

    [Authorize(Roles = "Dealer")]
    [HttpPut("{inquiryId}")]
    public async Task<IActionResult> UpdateInquiry(Guid inquiryId, UpdateInquiryRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _inquiryService.UpdateInquiryAsync(inquiryId, request, userId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Inquiry updated successfully!"
        });
    }
}
