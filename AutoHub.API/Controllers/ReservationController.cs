using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Reservation;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/reservation")]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;
    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [Authorize(Roles = "Buyer")]
    [HttpPost]
    public async Task<IActionResult> CreateReservation(CreateReservationRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var reservation = await _reservationService.CreateReservationAsync(request, userId);

        return Ok(new ApiResponse<CreateReservationResponse>
        {
            Success = true,
            Message = "Reservation has been created successfully.",
            Data = reservation
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{reservationId}")]
    public async Task<IActionResult> DeleteReservation(Guid reservationId)
    {
        await _reservationService.CancelReservationAsync(reservationId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Reservation has been cancelled."
        });
    }

    [Authorize(Roles = "Dealer")]
    [HttpGet("dealer/my")]
    public async Task<IActionResult> GetDealerReservations(
        [FromQuery] ReservationStatus? Status,
        [FromQuery] int PageNumber = 1, 
        [FromQuery] int Pagesize = 20)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var response = await _reservationService.GetDealerReservationsAsync(userId, Status, PageNumber, Pagesize);

        return Ok(new ApiResponse<PaginatedResponse<ReservationResponse>>
        {
            Success = true,
            Message = "Reservation for a dealer retrieved successfully.",
            Data = response
        });
    }

    [Authorize(Roles = "Buyer")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyReservations([FromQuery] ReservationStatus? Status)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var response = await _reservationService.GetMyReservationsAsync(userId, Status);

        return Ok(new ApiResponse<List<ReservationResponse>>
        {
            Success = true,
            Message = "your reservations retrieved successfully.",
            Data = response
        });
    }

    [Authorize]
    [HttpGet("{reservationId}")]
    public async Task<IActionResult> GetReservationById(Guid reservationId)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)!);

        var response = await _reservationService.GetReservationByIdAsync(reservationId);

        return Ok(new ApiResponse<ReservationResponse>
        {
            Success = true,
            Message = "Reservation retrieved successfully.",
            Data = response
        });
    }
}