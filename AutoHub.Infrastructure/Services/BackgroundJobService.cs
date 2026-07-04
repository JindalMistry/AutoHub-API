using AutoHub.Application.Interfaces;
using AutoHub.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly ApplicationDbcontext _dbcontext;
    private readonly ILogger<BackgroundJobService> _logger;
    private readonly ICacheService _cacheService;

    public BackgroundJobService(
        ApplicationDbcontext dbcontext, 
        ILogger<BackgroundJobService> logger, 
        ICacheService cacheService)
    {
        _dbcontext = dbcontext;
        _logger = logger;
        _cacheService = cacheService;
    }
    public async Task ExpireReservationsAsync()
    {
        var expiredReservations = await _dbcontext.Reservations
            .Include(o => o.Vehicle)
            .Where(o => o.Status == ReservationStatus.Active && o.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        if (expiredReservations.Count == 0) return;

        foreach (var reservation in expiredReservations)
        {
            reservation.Status = ReservationStatus.Expired;
            reservation.Vehicle.Status = VehicleStatus.Published;
        }

        _logger.LogInformation("{Count} reservations expired.", expiredReservations.Count);

        await _dbcontext.SaveChangesAsync();
    }

    public async Task FlushAnalyticsCountersAsync()
    {
        var analyticsRecords = await _dbcontext.Analytics
            .ToListAsync();

        foreach (var analytics in analyticsRecords)
        {
            var vehicleId = analytics.VehicleId;

            var viewKey = $"vehicle:{vehicleId}:views";
            var favouriteKey = $"vehicle:{vehicleId}:favourites";
            var inquiryKey = $"vehicle:{vehicleId}:inquiries";
            var reservationKey = $"vehicle:{vehicleId}:reservations";

            var views =
            await _cacheService
                .GetLongAsync(viewKey);

            var favourites =
                await _cacheService
                    .GetLongAsync(favouriteKey);

            var inquiries =
                await _cacheService
                    .GetLongAsync(inquiryKey);

            var reservations =
                await _cacheService
                    .GetLongAsync(reservationKey);

            analytics.ViewCount += (int)(views ?? 0);
            analytics.FavoriteCount += (int)(favourites ?? 0);
            analytics.InquiryCount += (int)(inquiries ?? 0);
            analytics.ReservationCount += (int)(reservations ?? 0);

            await _cacheService.RemoveAsync(viewKey);

            await _cacheService.RemoveAsync(favouriteKey);

            await _cacheService.RemoveAsync(inquiryKey);

            await _cacheService.RemoveAsync(reservationKey);

            await _dbcontext.SaveChangesAsync();
        }
    }

    public async Task RecalculateTrendingScoresAsync()
    {
        var analytics = await _dbcontext.Analytics.ToListAsync();

        if (analytics.Count == 0)
            return;

        var currentTime = DateTime.UtcNow;

        foreach (var item in analytics)
        {
            item.TrendingScore =
                item.ViewCount
                + (item.FavoriteCount * 5)
                + (item.InquiryCount * 10)
                + (item.ReservationCount * 20);

            item.LastCalculatedAt = currentTime;
        }

        await _dbcontext.SaveChangesAsync();

        await _cacheService.RemoveAsync("trending-vehicles");

        _logger.LogInformation(
            "{Count} analytics records recalculated.",
            analytics.Count);
    }
}
