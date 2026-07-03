using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using AutoHub.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbcontext _dbcontext;

    private readonly ICacheService _cacheService;

    public AnalyticsService(ApplicationDbcontext dbcontext, ICacheService cacheService)
    {
        _dbcontext = dbcontext;
        _cacheService = cacheService;
    }
    public async Task<List<VehicleListingResponse>> GetTrendingVehiclesAsync()
    {
        var cachedData = await _cacheService.GetAsync<List<VehicleListingResponse>>("trending-vehicles");

        if (cachedData != null)
        {
            return cachedData;
        }

        var response = await _dbcontext.Vehicles
            .AsNoTracking()
            .Where(o => o.Status == VehicleStatus.Published)
            .OrderByDescending(o => o.Analytics.TrendingScore)
            .Select(o => new VehicleListingResponse
            {
                Id = o.Id,
                Title = o.Title,
                Make = o.Make,
                Model = o.Model,
                Variant = o.Variant,
                Price = o.Price,
                Year = o.Year,
                Mileage = o.Mileage,
                Transmission = o.Transmission.ToString(),

                ThumbnailUrl = o.Images
                        .OrderBy(i =>
                            i.DisplayOrder)
                        .Select(i =>
                            i.ImageUrl)
                        .FirstOrDefault()
            })
            .Take(10)
            .ToListAsync();

        await _cacheService.SetAsync<List<VehicleListingResponse>>("trending-vehicles", response, TimeSpan.FromHours(1));

        return response;
    }
}
