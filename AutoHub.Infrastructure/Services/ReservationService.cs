using AutoHub.Application.DTOs.Reservation;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Exceptions;
using AutoHub.Application.Interfaces;
using AutoHub.Domain.Entities;
using AutoHub.Infrastructure.Migrations;
using AutoHub.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Services;

public class ReservationService : IReservationService
{
    private readonly ApplicationDbcontext _dbcontext;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ReservationService> _logger;
    private readonly IStorageService _storageService;

    public ReservationService(
        ApplicationDbcontext dbcontext, 
        ICacheService cacheService,
        ILogger<ReservationService> logger,
        IStorageService storageService)
    {
        _dbcontext = dbcontext;
        _cacheService = cacheService;
        _logger = logger;
        _storageService = storageService;
    }

    public async Task CancelReservationAsync(Guid reservationId)
    {
        var res = await _dbcontext.Reservations
            .FirstOrDefaultAsync(
            o => o.Id == reservationId);

        if (res == null) throw new NotFoundException("Reservation does not exist!");

        if (res.Status != ReservationStatus.Active)
        {
            throw new BadRequestException("Reservation is not active!");
        }

        res.Status = ReservationStatus.Cancelled;
        res.Vehicle.Status = VehicleStatus.Published;

        await _dbcontext.SaveChangesAsync();
    }

    public async Task<CreateReservationResponse> CreateReservationAsync(CreateReservationRequest request, Guid buyerId)
    {
        await using var transaction = await _dbcontext.Database.BeginTransactionAsync();

        try
        {
            var vehicle = await _dbcontext.Vehicles
            .FirstOrDefaultAsync(o =>
                o.Id == request.VehicleId);

            if (vehicle == null)
                throw new NotFoundException(
                    "Vehicle does not exist!");

            if (vehicle.Status == VehicleStatus.Sold)
            {
                throw new BadRequestException(
                    "Vehicle is sold, not available for reservation.");
            }

            if (vehicle.Status == VehicleStatus.Reserved)
            {
                throw new BadRequestException(
                    "Vehicle is reserved already!");
            }

            if (vehicle.Status != VehicleStatus.Published)
            {
                throw new BadRequestException(
                    "Vehicle is not published, please contact dealer for more information!");
            }


            var activeReservations =
            await _dbcontext.Reservations
                .CountAsync(o =>
                    o.UserId == buyerId &&
                    o.Status == ReservationStatus.Active);

            if (activeReservations >= 3)
                throw new BadRequestException(
                    "Maximum active reservations reached.");

            var reservation = new Reservation
                {
                    Id = Guid.NewGuid(),
                    VehicleId = vehicle.Id,
                    UserId = buyerId,
                    Status = ReservationStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(3)
                };

            vehicle.Status = VehicleStatus.Reserved;

            await _dbcontext.Reservations
                .AddAsync(reservation);

            await _dbcontext.SaveChangesAsync();

            await transaction.CommitAsync();

            _logger.LogInformation(
                "User {UserId} reserved vehicle {VehicleId}.",
                buyerId,
                vehicle.Id);

            await _cacheService.IncrementAsync($"vehicle:{vehicle.Id}:reservations");

            return new CreateReservationResponse
            {
                Id = reservation.Id
            };
        }
        catch(DbUpdateException)
        {
            await transaction.RollbackAsync();

            throw new BadRequestException(
                "Vehicle is already reserved.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PaginatedResponse<ReservationResponse>> GetDealerReservationsAsync(Guid userId, ReservationStatus? status, int pageNumber, int pageSize)
    {
        pageSize = Math.Min(pageSize, 50);

        var dealer = await _dbcontext.Dealers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == userId);

        if (dealer == null) throw new NotFoundException("Dealer does not exist!");

        var query = _dbcontext.Reservations
            .AsNoTracking()
            .Where(o => o.Vehicle.DealerId == dealer.Id);

        if (status.HasValue)
        {
            query = query
                .Where(o => o.Status == status);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new ReservationResponse
            {
                Id = o.Id,
                VehicleId = o.VehicleId,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ExpiresAt = o.ExpiresAt,
                BuyerName = o.User.Name,
                BuyerEmail = o.User.Email,
                Vehicle = new VehicleListingResponse
                {
                    Id = o.Vehicle.Id,
                    Title = o.Vehicle.Title,
                    Make = o.Vehicle.Make,
                    Model = o.Vehicle.Model,
                    Variant = o.Vehicle.Variant,
                    Price = o.Vehicle.Price,
                    Year = o.Vehicle.Year,
                    Mileage = o.Vehicle.Mileage,
                    Transmission = o.Vehicle.Transmission.ToString(),
                    ThumbnailUrl = o.Vehicle.Images
                        .OrderBy(i =>
                            i.DisplayOrder)
                        .Select(i =>
                            i.ImageUrl)
                        .FirstOrDefault()
                }
            })
            .ToListAsync();

        foreach (var data in items)
        {
            var url = data.Vehicle.ThumbnailUrl;

            if (url == null) continue;

            data.Vehicle.ThumbnailUrl = await _storageService.GetPresignedUrlAsync(url, TimeSpan.FromMinutes(60));
        }

        return new PaginatedResponse<ReservationResponse>
        {
            TotalRecords = total,
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }

    public async Task<List<ReservationResponse>> GetMyReservationsAsync(Guid buyerId, ReservationStatus? status)
    {
        var query = _dbcontext.Reservations
            .AsNoTracking()
            .Where(o => o.UserId == buyerId);

        if (status.HasValue)
        {
            query = query
                .Where(o => o.Status == status);
        }

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new ReservationResponse
            {
                Id = o.Id,
                VehicleId = o.VehicleId,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ExpiresAt = o.ExpiresAt,
                Vehicle = new VehicleListingResponse
                {
                    Id = o.Vehicle.Id,
                    Title = o.Vehicle.Title,
                    Make = o.Vehicle.Make,
                    Model = o.Vehicle.Model,
                    Variant = o.Vehicle.Variant,
                    Price = o.Vehicle.Price,
                    Year = o.Vehicle.Year,
                    Mileage = o.Vehicle.Mileage,
                    Transmission = o.Vehicle.Transmission.ToString(),
                    ThumbnailUrl = o.Vehicle.Images
                        .OrderBy(i =>
                            i.DisplayOrder)
                        .Select(i =>
                            i.ImageUrl)
                        .FirstOrDefault()
                }
            })
            .ToListAsync();

        foreach (var data in items)
        {
            var url = data.Vehicle.ThumbnailUrl;

            if (url == null) continue;

            data.Vehicle.ThumbnailUrl = await _storageService.GetPresignedUrlAsync(url, TimeSpan.FromMinutes(60));
        }

        return items;
    }

    public async Task<ReservationResponse> GetReservationByIdAsync(Guid reservationId)
    {
        var reservation = await _dbcontext.Reservations
            .AsNoTracking()
            .Include(o => o.Vehicle)
            .FirstOrDefaultAsync(o => o.Id == reservationId);

        if (reservation == null) throw new BadRequestException("Reservation does not exist!");

        var response = new ReservationResponse
        {
            Id = reservation.Id,
            VehicleId = reservation.VehicleId,
            Status = reservation.Status,
            CreatedAt = reservation.CreatedAt,
            ExpiresAt = reservation.ExpiresAt,
            Vehicle = new VehicleListingResponse
            {
                Id = reservation.Vehicle.Id,
                Title = reservation.Vehicle.Title,
                Make = reservation.Vehicle.Make,
                Model = reservation.Vehicle.Model,
                Variant = reservation.Vehicle.Variant,
                Price = reservation.Vehicle.Price,
                Year = reservation.Vehicle.Year,
                Mileage = reservation.Vehicle.Mileage,
                Transmission = reservation.Vehicle.Transmission.ToString(),
                ThumbnailUrl = reservation.Vehicle.Images
                        .OrderBy(i =>
                            i.DisplayOrder)
                        .Select(i =>
                            i.ImageUrl)
                        .FirstOrDefault()
            }
        };

        var url = response.Vehicle.ThumbnailUrl;

        if(url != null) response?.Vehicle.ThumbnailUrl = await _storageService.GetPresignedUrlAsync(url, TimeSpan.FromHours(1));

        return response;
    }
}
