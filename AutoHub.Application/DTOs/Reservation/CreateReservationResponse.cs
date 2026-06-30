using AutoHub.Application.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Reservation;

public class CreateReservationResponse
{
    public Guid Id { get; set; }
}

public class ReservationResponse
{
    public Guid Id { get; set; }

    public Guid VehicleId { get; set; }

    public required VehicleListingResponse Vehicle { get; set; }

    public ReservationStatus Status { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string BuyerName { get; set; } = string.Empty;

    public string BuyerEmail { get; set; } = string.Empty;

    public string BuyerPhone { get; set; } = string.Empty;
}