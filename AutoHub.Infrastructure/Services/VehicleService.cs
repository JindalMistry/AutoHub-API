using AutoHub.Application.DTOs.Dealers;
using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Exceptions;
using AutoHub.Application.Interfaces;
using AutoHub.Domain.Entities;
using AutoHub.Domain.Enums;
using AutoHub.Infrastructure.Persistance;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;

namespace AutoHub.Infrastructure.Services;

public class VehicleService : IVehicleService
{
    private readonly ApplicationDbcontext _dbcontext;
    private readonly ICacheService _cacheService;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(
        ApplicationDbcontext dbcontext, 
        ICacheService cacheService, 
        ILogger<VehicleService> logger)
    {
        _dbcontext = dbcontext;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<VehicleResponse> CreateVehicleAsync(CreateVehicleRequest request, Guid userId)
    {
        var dealer = await _dbcontext.Dealers.FirstOrDefaultAsync(x => x.UserId == userId);

        if (dealer == null)
        {
            throw new NotFoundException("Dealer does not exist!");
        }

        if (dealer.Status != DealerStatus.Approved)
        {
            throw new ForbiddenException("Dealer profile is not approved, Contact admin for more details!");
        }

        var existingVehicle = await _dbcontext.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.RegNo == request.RegNo);

        if (existingVehicle != null) throw new BadRequestException("Vehicle with same Registration exists!");

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            DealerId = dealer.Id,
            Title = request.Title,
            RegNo = request.RegNo,
            Price = request.Price,
            Description = request.Description,
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Variant = request.Variant,
            Mileage = request.Mileage,
            FuelType = request.FuelType,
            Transmission = request.Transmission,
            Status = VehicleStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbcontext.Vehicles.Add(vehicle);

        var vehicleAnalytics = new VehicleAnalytics
        {
            VehicleId = vehicle.Id,
            ViewCount = 0,
            FavoriteCount = 0,
            InquiryCount = 0,
            ReservationCount = 0,
            LastCalculatedAt = DateTime.UtcNow,
            TrendingScore = 0
        };

        _dbcontext.Analytics.Add(vehicleAnalytics);

        await _dbcontext.SaveChangesAsync();

        _logger.LogInformation(
            "Dealer {DealerId} created vehicle {VehicleId}.",
            dealer.Id,
            vehicle.Id);

        return new VehicleResponse
        {
            Id = vehicle.Id,
            Title = vehicle.Title,
            Price = vehicle.Price,
            Status = vehicle.Status.ToString()
        };
    }

    public async Task DeleteVehicleAsync(Guid vehicleId, Guid userId)
    {
        var dealer = await _dbcontext.Dealers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == userId);

        if (dealer == null) throw new NotFoundException("Dealer does not exist!");

        if (dealer.Status != DealerStatus.Approved) throw new ForbiddenException("Dealer is not approved!");

        var vehicle = await _dbcontext.Vehicles
            .FirstOrDefaultAsync(o => o.Id == vehicleId && o.DealerId == dealer.Id);

        if (vehicle == null) throw new NotFoundException("Vehicle does not exist!");

        if (vehicle.Status != VehicleStatus.Draft) throw new ForbiddenException("Only draft vehicles can be deleted!");

        var vehicleAnalytics = await _dbcontext.Analytics
            .FirstOrDefaultAsync(o => o.VehicleId == vehicleId);

        if (vehicleAnalytics != null) _dbcontext.Analytics.Remove(vehicleAnalytics);

        _dbcontext.Vehicles.Remove(vehicle);

        await _dbcontext.SaveChangesAsync();

        var cachedKey = $"vehicle:{vehicle.Id}";
        await _cacheService.RemoveAsync(cachedKey);

        await _cacheService.RemoveAsync("vehicle-filter-options");
    }

    public async Task<List<VehicleResponse>> GetMyVehiclesAsync(Guid userId)
    {
        var dealer = await _dbcontext.Dealers.FirstOrDefaultAsync(o => o.UserId == userId);

        if (dealer == null)
        {
            throw new NotFoundException("Dealer does not exist!");
        }

        if (dealer.Status != DealerStatus.Approved)
        {
            throw new ForbiddenException("Dealer is not approved!");
        }

        var vehicles = await _dbcontext.Vehicles
            .AsNoTracking()
            .Where(o => o.DealerId == dealer.Id)
            .Select(o => new VehicleResponse
            {
                Id = o.Id,
                Title = o.Title,
                Price = o.Price,
                Status = o.Status.ToString(),
                Make = o.Make,
                Model = o.Model,
                Variant = o.Variant,
                Mileage = o.Mileage,
                Year = o.Year,
                Transmission = o.Transmission.ToString(),
                FuelType = o.FuelType.ToString(),
                ThumbnailUrl = o.Images
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => o.ImageUrl)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return vehicles;
    }

    public async Task<VehicleResponse> GetVehicleByIdAsync(Guid vehicleId, Guid? userId)
    {
        async Task<bool> IsFavourite(Guid vId, Guid? uId)
        {
            return await _dbcontext.Favourites.AnyAsync(o => o.UserId == uId&& o.VehicleId == vId);
        }

        var cacheKey = $"vehicle:{vehicleId}";

        var cachedData = await _cacheService.GetAsync<VehicleResponse>(cacheKey);

        if (cachedData != null) 
        {
            await _cacheService.IncrementAsync($"vehicle:{vehicleId}:views");

            if (userId.HasValue) cachedData.IsFavourite = await IsFavourite(vehicleId, userId);

            return cachedData;
        }
        
        var vehicle = await _dbcontext.Vehicles
            .AsNoTracking()
            .Include(o => o.Dealer)
            .FirstOrDefaultAsync(o => o.Id == vehicleId);

        if (vehicle == null)
        {
            throw new NotFoundException("Vehicle does not exist!");
        }

        if (vehicle.Status == VehicleStatus.Draft)
        {
            throw new NotFoundException(
                "Vehicle does not exist!");
        }

        await _cacheService.IncrementAsync($"vehicle:{vehicleId}:views");

        var response = new VehicleResponse
        {
            Id = vehicle.Id,
            Title = vehicle.Title,
            Price = vehicle.Price,
            Status = vehicle.Status.ToString(),
            Make = vehicle.Make,
            Model = vehicle.Model,
            Variant = vehicle.Variant,
            Mileage = vehicle.Mileage,
            Year = vehicle.Year,
            Transmission = vehicle.Transmission.ToString(),
            FuelType = vehicle.FuelType.ToString(),
            Dealer = new DealerResponse
            {
                BusinessName = vehicle.Dealer.BusinessName,
                City = vehicle.Dealer.City,
                Country = vehicle.Dealer.Country,
                Id = vehicle.DealerId,
                Phone = vehicle.Dealer.Phone,
                Pincode = vehicle.Dealer.Pincode,
                Status = vehicle.Dealer.Status
            },
            IsFavourite = false
        };

        await _cacheService.SetAsync<VehicleResponse>(cacheKey, response, TimeSpan.FromMinutes(10));

        var IsFav = false;
        
        if (userId.HasValue) IsFav = await IsFavourite(vehicleId, userId);

        response.IsFavourite = IsFav;

        return response;
    }

    public async Task<VehicleResponse> GetAnyVehicleAsync(Guid vehicleId)
    {
        var vehicle = await _dbcontext.Vehicles
            .AsNoTracking()
            .Include(o => o.Dealer)
            .FirstOrDefaultAsync(o => o.Id == vehicleId);

        if (vehicle == null)
        {
            throw new NotFoundException("Vehicle does not exist!");
        }

        var response = new VehicleResponse
        {
            Id = vehicle.Id,
            Title = vehicle.Title,
            Price = vehicle.Price,
            Status = vehicle.Status.ToString(),
            Make = vehicle.Make,
            Model = vehicle.Model,
            Variant = vehicle.Variant,
            Mileage = vehicle.Mileage,
            Year = vehicle.Year,
            Transmission = vehicle.Transmission.ToString(),
            FuelType = vehicle.FuelType.ToString(),
            Dealer = new DealerResponse
            {
                BusinessName = vehicle.Dealer.BusinessName,
                City = vehicle.Dealer.City,
                Country = vehicle.Dealer.Country,
                Id = vehicle.DealerId,
                Phone = vehicle.Dealer.Phone,
                Pincode = vehicle.Dealer.Pincode,
                Status = vehicle.Dealer.Status
            }
        };

        return response;
    }

    public async Task PublishVehicleAsync(Guid vehicleId, Guid userId)
    {
        var vehicle = await _dbcontext.Vehicles
            .FirstOrDefaultAsync(o => o.Id == vehicleId);

        if (vehicle == null) throw new NotFoundException("Vehicle does not exist!");

        if (vehicle.Status != VehicleStatus.Draft) throw new ForbiddenException("Only draft vehicles can be published!");

        var hasImages = await _dbcontext.VehicleImages
            .AsNoTracking()
            .AnyAsync(o => o.VehicleId == vehicleId);

        if (!hasImages) throw new BadRequestException("Vehicle must have atleast one image before publishing!");

        vehicle.Status = VehicleStatus.Published;
        vehicle.UpdatedAt = DateTime.UtcNow;
        vehicle.ApprovedByUserId = userId;

        await _dbcontext.SaveChangesAsync();

        _logger.LogInformation(
            "Admin {AdminId} published vehicle {VehicleId}.",
            userId,
            vehicle.Id);

        var cachedKey = $"vehicle:{vehicleId}";
        await _cacheService.RemoveAsync(cachedKey);

        await _cacheService.RemoveAsync("vehicle-filter-options");
    }

    public async Task UnpublishVehicleAsync(Guid vehicleId, Guid adminId)
    {
        var vehicle = await _dbcontext.Vehicles
            .FirstOrDefaultAsync(o => o.Id == vehicleId);

        if (vehicle == null)
        {
            throw new NotFoundException(
                "Vehicle does not exist!");
        }

        if (vehicle.Status != VehicleStatus.Published)
        {
            throw new ForbiddenException(
                "Only published vehicles can be moved back to draft");
        }

        vehicle.Status = VehicleStatus.Draft;
        vehicle.UpdatedAt = DateTime.UtcNow;
        vehicle.ApprovedByUserId = null;

        await _dbcontext.SaveChangesAsync();

        _logger.LogInformation(
            "Admin {AdminId} published vehicle {VehicleId}.",
            adminId,
            vehicle.Id);

        var cachedKey = $"vehicle:{vehicleId}";
        await _cacheService.RemoveAsync(cachedKey);

        await _cacheService.RemoveAsync("vehicle-filter-options");
    }

    public async Task<VehicleResponse> UpdateVehicleAsync(CreateVehicleRequest request, Guid userId, Guid vehicleId)
    {
        var dealer = await _dbcontext.Dealers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == userId);

        if (dealer == null) throw new NotFoundException("Dealer does not exist!");

        if (dealer.Status != DealerStatus.Approved) throw new ForbiddenException("Dealer is not approved");

        var vehicle = await _dbcontext.Vehicles
            .FirstOrDefaultAsync(o => o.Id == vehicleId && o.DealerId == dealer.Id);

        if (vehicle == null) throw new NotFoundException("Vehicle not found!");

        if (vehicle.Status != VehicleStatus.Draft) throw new ForbiddenException("Can not update vehicle as it has been published");

        vehicle.Title = request.Title;
        vehicle.RegNo = request.RegNo;
        vehicle.Price = request.Price;
        vehicle.Description = request.Description;
        vehicle.Make = request.Make;
        vehicle.Model = request.Model;
        vehicle.Year = request.Year;
        vehicle.Variant = request.Variant;
        vehicle.Mileage = request.Mileage;
        vehicle.FuelType = request.FuelType;
        vehicle.Transmission = request.Transmission;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _dbcontext.SaveChangesAsync();

        var cachedKey = $"vehicle:{vehicleId}";
        await _cacheService.RemoveAsync(cachedKey);

        return new VehicleResponse
        {
            Id = vehicle.Id,
            Title = vehicle.Title,
            Price = vehicle.Price,
            Status = vehicle.Status.ToString(),
            Make = vehicle.Make,
            Model = vehicle.Model,
            Variant = vehicle.Variant
        };
    }

    public async Task<PaginatedResponse<VehicleListingResponse>> SearchVehiclesAsync(VehicleSearchRequest request)
    {
        //Cap page size to 50
        request.PageSize = Math.Min(request.PageSize, 50);

        var query = _dbcontext.Vehicles
            .AsNoTracking()
            .Where(o =>
                o.Status == VehicleStatus.Published);

        // Search
        if (!string.IsNullOrWhiteSpace(
            request.SearchTerm))
        {
            query = query.Where(o =>
                EF.Functions.ILike(
                    o.Title,
                    $"%{request.SearchTerm}%")
                ||
                EF.Functions.ILike(
                    o.Make,
                    $"%{request.SearchTerm}%")
                ||
                EF.Functions.ILike(
                    o.Model,
                    $"%{request.SearchTerm}%"));
        }

        // Make
        if (!string.IsNullOrWhiteSpace(
            request.Make))
        {
            query = query.Where(o => 
                EF.Functions.ILike(
                    o.Make, 
                    request.Make));
        }

        // Model
        if (!string.IsNullOrWhiteSpace(
            request.Model))
        {
            query = query.Where(o =>
                EF.Functions.ILike(
                    o.Model,
                    request.Model));
        }

        // Variant
        if (!string.IsNullOrWhiteSpace(
            request.Variant))
        {
            query = query.Where(o =>
                EF.Functions.ILike(
                    o.Variant,
                    request.Variant));
        }

        // Price
        if (request.MinPrice.HasValue)
        {
            query = query.Where(o =>
                o.Price >= request.MinPrice);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(o =>
                o.Price <= request.MaxPrice);
        }

        // Year
        if (request.MinYear.HasValue)
        {
            query = query.Where(o =>
                o.Year >= request.MinYear);
        }

        if (request.MaxYear.HasValue)
        {
            query = query.Where(o =>
                o.Year <= request.MaxYear);
        }

        // Fuel
        if (request.FuelType.HasValue)
        {
            query = query.Where(o =>
                o.FuelType == request.FuelType);
        }

        // Transmission
        if (request.Transmission.HasValue)
        {
            query = query.Where(o =>
                o.Transmission ==
                request.Transmission);
        }

        // Sorting
        query = request.SortBy switch
        {
            VehicleSortBy.PriceAsc =>
                query.OrderBy(o => o.Price),

            VehicleSortBy.PriceDesc =>
                query.OrderByDescending(o => o.Price),

            VehicleSortBy.YearDesc =>
                query.OrderByDescending(o => o.Year),

            _ =>
                query.OrderByDescending(
                    o => o.CreatedAt)
        };

        var totalRecords =
            await query.CountAsync();

        var vehicles = await query
            .Skip(
                (request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
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

                    ThumbnailUrl = o.Images
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
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages =
                (int)Math.Ceiling(
                    totalRecords /
                    (double)request.PageSize)
        };
    }

    public async Task<VehicleFilterOptionsResponse> GetVehicleFilterOptionsAsync()
    {
        var cachedData = await _cacheService.GetAsync<VehicleFilterOptionsResponse>("vehicle-filter-options");

        if (cachedData != null) return cachedData;

        var makes = await _dbcontext.Vehicles
            .AsNoTracking()
            .Where(o => o.Status == VehicleStatus.Published)
            .Select(o => o.Make)
            .Distinct()
            .OrderBy(o => o)
            .ToListAsync();

        var models = await _dbcontext.Vehicles
            .AsNoTracking()
            .Where(o => o.Status == VehicleStatus.Published)
            .Select(o => o.Model)
            .Distinct()
            .OrderBy(o => o)
            .ToListAsync();

        var variants = await _dbcontext.Vehicles
            .AsNoTracking()
            .Where(o => o.Status == VehicleStatus.Published)
            .Select(o => o.Variant)
            .Distinct()
            .OrderBy(o => o)
            .ToListAsync();

        var fuelTypes =
            Enum.GetValues<FuelType>()
                .Select(o => new EnumOptionResponse
                {
                    Value = (int)o,
                    Name = o.ToString()
                })
                .ToList();

        var transmissions =
            Enum.GetValues<TransmissionType>()
                .Select(o => new EnumOptionResponse
                {
                    Value = (int)o,
                    Name = o.ToString()
                })
                .ToList();

        var minPrice =
            await _dbcontext.Vehicles
                .MinAsync(o => o.Price);

        var maxPrice =
            await _dbcontext.Vehicles
                .MaxAsync(o => o.Price);

        var response = new VehicleFilterOptionsResponse
        {
            Makes = makes,
            Models = models,
            Variants = variants,
            FuelTypes = fuelTypes,
            Transmissions = transmissions,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };

        await _cacheService.SetAsync<VehicleFilterOptionsResponse>("vehicle-filter-options", response, TimeSpan.FromHours(24));

        return response;
    }
}