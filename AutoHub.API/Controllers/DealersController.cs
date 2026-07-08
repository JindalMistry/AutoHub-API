using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Dealers;
using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/dealers")]
public class DealersController : ControllerBase
{
    private readonly IDealerService _dealerService;
    public DealersController (IDealerService dealerService) 
    { 
        _dealerService = dealerService;             
    }

    [Authorize]
    [HttpPost("apply")]
    public async Task<IActionResult> ApplyDealer(CreateDealerProfileRequest createDealerProfileRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var response = await _dealerService.CreateDealerProfileAsync(createDealerProfileRequest, userId);

        return Ok(new ApiResponse<DealerResponse>
        {
            Success = true,
            Message = "Dealer has been created, Contact admin for approval.",
            Data = response
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveDealer(Guid id)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _dealerService.ApproveDealerAsync(id, adminId);

        return Ok(new ApiResponse<object>
        {
            Message = "Dealer has been approved.",
            Success = true
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingDealers()
    {
        var response = await _dealerService.GetPendingDealersAsync();

        return Ok(new ApiResponse<List<DealerResponse>>
        {
            Success = true,
            Message = "Pending dealers retrieved successfully",
            Data = response
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectDealer(Guid id)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _dealerService.RejectDealerAsync(id, adminId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Dealer has been rejected!",
        });
    }
}
