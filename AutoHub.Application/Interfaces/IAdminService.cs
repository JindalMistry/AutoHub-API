using AutoHub.Application.DTOs.Admin;
using AutoHub.Application.DTOs.Vehicles;

namespace AutoHub.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardResponse> GetDashboardAsync();

    Task<List<TopVehiclesResponse>> GetTopVehiclesAsync();

    Task<List<TopDealerResponse>> GetTopDealersAsync();

    Task<List<PendingDealerResponse>> GetPendingDealerAsync();

    Task<List<CompletedReservationResponse>> GetActiveReservationsAsync();

    Task<List<VehicleListingResponse>> GetPendingVehiclesAsync();
}
