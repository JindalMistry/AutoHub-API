using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Admin;

public class AdminDashboardResponse
{
    public long TotalUsers { get; set; } = 0;
    public long TotalDealers { get; set; } = 0;
    public long TotalVehicles { get; set; } = 0;
    public long PublishedVehicles { get; set; } = 0;
    public long ReservedVehicles { get; set; } = 0;
    public long SoldVehicles { get; set; } = 0;
    public long TotalFavourites { get; set; } = 0;
    public long TotalInquiries { get; set; } = 0;
    public long TotalReservations { get; set; } = 0;
    public long TotalVehicleViews { get; set; } = 0;
}
