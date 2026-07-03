using AutoHub.Application.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.Interfaces;

public interface IAnalyticsService
{
    Task<List<VehicleListingResponse>> GetTrendingVehiclesAsync();
}
