using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Admin;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Interfaces;
using AutoHub.Domain.Enums;
using AutoHub.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbcontext _dbcontext;

    public AdminService(ApplicationDbcontext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public async Task<AdminDashboardResponse> GetDashboardAsync()
    {
        var totalUsers = await _dbcontext.Users
            .AsNoTracking()
            .CountAsync(o => o.IsActive == true && o.Role == UserRole.Buyer);

        var totalDealers = await _dbcontext.Users
            .AsNoTracking()
            .CountAsync(o => o.IsActive == true && o.Role == UserRole.Dealer);

        var vehicleQuery = _dbcontext.Vehicles.AsNoTracking();

        var totalVehicles = await vehicleQuery
            .CountAsync();

        var reservedVehicles = await vehicleQuery
            .Where(o => o.Status == VehicleStatus.Reserved)
            .CountAsync();

        var publishedVehicles = await vehicleQuery
            .Where(o => o.Status == VehicleStatus.Published)
            .CountAsync();

        var soldVehicles = await vehicleQuery
            .Where(o => o.Status == VehicleStatus.Sold)
            .CountAsync();

        var analyticsQuery = _dbcontext.Analytics.AsNoTracking();

        var totalFavourites = await analyticsQuery
            .SumAsync(o => (long?)o.FavoriteCount ?? 0);

        var totalViews = await analyticsQuery
            .SumAsync(o => (long?)o.ViewCount) ?? 0;

        var totalInquiries = await analyticsQuery
           .SumAsync(o => (long?)o.InquiryCount) ?? 0;

        var totalReservations = await analyticsQuery
            .SumAsync(o => (long?)o.ReservationCount) ?? 0;

        return new AdminDashboardResponse
        {
            TotalUsers = totalUsers,
            TotalDealers = totalDealers,
            TotalVehicles = totalVehicles,
            PublishedVehicles = publishedVehicles,
            ReservedVehicles = reservedVehicles,
            SoldVehicles = soldVehicles,
            TotalFavourites = totalFavourites,
            TotalInquiries = totalInquiries,
            TotalReservations = totalReservations,
            TotalVehicleViews = totalViews
        };
    }

    public async Task<List<TopVehiclesResponse>> GetTopVehiclesAsync()
    {
        return await _dbcontext.Vehicles
            .AsNoTracking()
            .Where(o => o.Status != VehicleStatus.Draft)
            .Select(o => new TopVehiclesResponse
            {
                Id = o.Id,
                Title = o.Title,
                Views = o.Analytics!.ViewCount,
                Favourites = o.Analytics!.FavoriteCount,
                Inquiries = o.Analytics!.InquiryCount,
                Reservations = o.Analytics!.ReservationCount,
                TrendingScore = (long)o.Analytics!.TrendingScore
            })
            .OrderByDescending(o => o.TrendingScore)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<TopDealerResponse>> GetTopDealersAsync()
    {
        return await _dbcontext.Dealers
            .AsNoTracking()
            .Where(o =>
                o.Status == DealerStatus.Approved)
            .Select(o =>
                new TopDealerResponse
                {
                    DealerId = o.Id,

                    BusinessName = o.BusinessName,

                    TotalVehicles =
                        o.Vehicles.Count(v =>
                            v.Status != VehicleStatus.Draft),

                    TotalViews =
                        o.Vehicles
                            .Where(v =>
                                v.Status != VehicleStatus.Draft)
                            .Sum(v =>
                                (long?)v.Analytics!.ViewCount)
                        ?? 0,

                    TotalFavourites =
                        o.Vehicles
                            .Where(v =>
                                v.Status != VehicleStatus.Draft)
                            .Sum(v =>
                                (long?)v.Analytics!.FavoriteCount)
                        ?? 0,

                    TotalInquiries =
                        o.Vehicles
                            .Where(v =>
                                v.Status != VehicleStatus.Draft)
                            .Sum(v =>
                                (long?)v.Analytics!.InquiryCount)
                        ?? 0,

                    TotalReservations =
                        o.Vehicles
                            .Where(v =>
                                v.Status != VehicleStatus.Draft)
                            .Sum(v =>
                                (long?)v.Analytics!.ReservationCount)
                        ?? 0,

                    TotalTrendingScore =
                        o.Vehicles
                            .Where(v =>
                                v.Status != VehicleStatus.Draft)
                            .Sum(v =>
                                (decimal?)v.Analytics!.TrendingScore)
                        ?? 0
                })
            .OrderByDescending(o =>
                o.TotalTrendingScore)
            .Take(10)
            .ToListAsync();
    }

    public async Task<List<PendingDealerResponse>> GetPendingDealerAsync()
    {
        var response = await _dbcontext.Dealers
            .AsNoTracking()
            .Where(o => o.Status == DealerStatus.Pending)
            .Select(o => new PendingDealerResponse
            {
                Id = o.Id,
                UserId = o.UserId,
                BusinessName = o.BusinessName,
                City = o.City,
                Country = o.Country,
                Phone = o.Phone,
                Pincode = o.Pincode
            })
            .ToListAsync();

        return response;
    }

    public async Task<List<CompletedReservationResponse>> GetActiveReservationsAsync()
    {
        var response = await _dbcontext.Reservations
            .AsNoTracking()
            .Where(o => o.Status == ReservationStatus.Completed)
            .Select(o => new CompletedReservationResponse
            {
                Id = o.Id,
                VehicleId = o.VehicleId,
                UserId = o.UserId,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ExpiresAt = o.ExpiresAt,
                Vehicle = new VehicleListingResponse
                {
                    Id = o.VehicleId,
                    DealerId = o.Vehicle.DealerId,
                    Make = o.Vehicle.Make,
                    Model = o.Vehicle.Model,
                    Price = o.Vehicle.Price,
                    ThumbnailUrl = o.Vehicle.Images
                            .OrderBy(x => x.DisplayOrder)
                            .Select(x => x.ImageUrl)
                            .FirstOrDefault(),
                    Title = o.Vehicle.Title,
                    Variant = o.Vehicle.Variant,
                    Year = o.Vehicle.Year,
                    Mileage = o.Vehicle.Mileage,
                    Transmission = o.Vehicle.Transmission.ToString()
                }
            })
            .ToListAsync();

        return response;
    }

    public async Task<List<VehicleListingResponse>> GetPendingVehiclesAsync()
    {
        var response = await _dbcontext.Vehicles
            .AsNoTracking()
            .Where(o => o.Status == VehicleStatus.Draft && o.ApprovedByUserId == null)
            .Select(o => new VehicleListingResponse
            {
                Id = o.Id,
                DealerId = o.DealerId,
                Make = o.Make,
                Model = o.Model,
                Price = o.Price,
                ThumbnailUrl = o.Images
                            .OrderBy(x => x.DisplayOrder)
                            .Select(x => x.ImageUrl)
                            .FirstOrDefault(),
                Title = o.Title,
                Variant = o.Variant,
                Year = o.Year,
                Mileage = o.Mileage,
                Transmission = o.Transmission.ToString()
            })
            .ToListAsync();
        return response!;
    }
}
