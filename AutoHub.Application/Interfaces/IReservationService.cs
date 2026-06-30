using AutoHub.Application.DTOs.Reservation;
using AutoHub.Application.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.Interfaces;

public interface IReservationService
{
    Task<CreateReservationResponse> CreateReservationAsync(
        CreateReservationRequest request,
        Guid buyerId);

    Task CancelReservationAsync(
        Guid reservationId);

    Task<List<ReservationResponse>>
        GetMyReservationsAsync(
            Guid buyerId,
            ReservationStatus? status);

    Task<ReservationResponse>
        GetReservationByIdAsync(
            Guid reservationId);

    Task<PaginatedResponse<ReservationResponse>>
        GetDealerReservationsAsync(
            Guid userId,
            ReservationStatus? status,
            int pageNumber,
            int pageSize);
}
