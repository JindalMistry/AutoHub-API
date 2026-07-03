using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Admin;

public class CompletedReservationResponse
{
    public Guid Id { get; set; }

    public Guid VehicleId { get; set; }

    public Guid UserId { get; set; }

    public ReservationStatus Status { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public required VehicleListingResponse Vehicle { get; set; }
}
