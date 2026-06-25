using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Exceptions;
using AutoHub.Application.Interfaces;
using AutoHub.Domain.Entities;
using AutoHub.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Services;

public class FavouriteService : IFavouriteService
{
    private readonly ApplicationDbcontext _dbcontext;
    private readonly ICacheService _cacheService;
    public FavouriteService(ApplicationDbcontext dbcontext, ICacheService cacheService)
    {
        _dbcontext = dbcontext;
        _cacheService = cacheService;
    }

    public async Task AddFavouriteAsync(Guid vehicleId, Guid userId)
    {
        var vehicle = await _dbcontext.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(o =>
                o.Id == vehicleId);

        if (vehicle == null)
            throw new NotFoundException(
                "Vehicle does not exist!");

        if (vehicle.Status != VehicleStatus.Published)
            throw new BadRequestException(
                "Only published vehicles can be added to favourites.");

        var existingFavourite =
            await _dbcontext.Favourites
                .AsNoTracking()
                .AnyAsync(o =>
                    o.VehicleId == vehicleId &&
                    o.UserId == userId);

        if (existingFavourite)
            throw new BadRequestException(
                "Vehicle already exists in favourites.");

        var favourite = new Favourite
        {
            UserId = userId,
            VehicleId = vehicleId,
            CreatedAt = DateTime.UtcNow
        };

        //var analytics = await _dbcontext.Analytics
        //    .FirstOrDefaultAsync(o => o.VehicleId == vehicleId);

        //if (analytics != null)
        //{
        //    analytics.FavoriteCount++;
        //}

        await _dbcontext.Favourites
            .AddAsync(favourite);

        await _dbcontext.SaveChangesAsync();

        await _cacheService.IncrementAsync($"vehicle:{vehicleId}:favourites");
    }

    public async Task RemoveFavouriteAsync(Guid vehicleId, Guid userId)
    {
        var favourite = await _dbcontext.Favourites
            .FirstOrDefaultAsync(o =>
                o.VehicleId == vehicleId &&
                o.UserId == userId);

        if (favourite == null)
            throw new NotFoundException(
                "Favourite does not exist.");

        //var analytics = await _dbcontext.Analytics
        //    .FirstOrDefaultAsync(o => o.VehicleId == vehicleId);

        //if (analytics != null)
        //{
        //    analytics.FavoriteCount = Math.Max(0, analytics.FavoriteCount - 1);
        //}

        _dbcontext.Favourites.Remove(
            favourite);

        await _dbcontext.SaveChangesAsync();

        await _cacheService.DecrementAsync($"vehicle:{vehicleId}:favourites");
    }

    public async Task<
    PaginatedResponse<VehicleListingResponse>>
    GetMyFavouritesAsync(Guid userId, int pageNumber, int pageSize)
    {
        pageSize = Math.Min(pageSize, 50);

        var query = _dbcontext.Favourites
            .AsNoTracking()
            .Where(o =>
                o.UserId == userId)
            .Select(o =>
                o.Vehicle);

        var totalRecords =
            await query.CountAsync();

        var vehicles = await query
            .OrderByDescending(o =>
                o.CreatedAt)
            .Skip(
                (pageNumber - 1)
                * pageSize)
            .Take(pageSize)
            .Select(o =>
                new VehicleListingResponse
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
                    ThumbnailUrl =
                        o.Images
                            .OrderBy(i =>
                                i.DisplayOrder)
                            .Select(i =>
                                i.ImageUrl)
                            .FirstOrDefault()
                })
            .ToListAsync();

        return new PaginatedResponse<VehicleListingResponse>
        {
            Items = vehicles,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages =
                (int)Math.Ceiling(
                    totalRecords /
                    (double)pageSize)
        };
    }
}