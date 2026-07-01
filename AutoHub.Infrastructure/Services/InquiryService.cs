using AutoHub.Application.DTOs.Inquiry;
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

public class InquiryService : IInquiryService
{
    private readonly ApplicationDbcontext _dbcontext;
    private readonly ICacheService _cacheService;

    public InquiryService(ApplicationDbcontext dbcontext, ICacheService cacheService)
    {
        _dbcontext = dbcontext;
        _cacheService = cacheService;
    }

    public async Task CreateInquiryAsync(CreateInquiryRequest request, Guid buyerId)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new BadRequestException(
                "Message is required.");
        }

        Guid dealerId;

        if (request.VehicleId.HasValue)
        {
            var vehicle = await _dbcontext.Vehicles
                .AsNoTracking()
                .FirstOrDefaultAsync(o =>
                    o.Id == request.VehicleId.Value);

            if (vehicle == null)
            {
                throw new NotFoundException(
                    "Vehicle does not exist.");
            }

            if (vehicle.Status != VehicleStatus.Published)
            {
                throw new BadRequestException(
                    "Inquiry can only be created for published vehicles.");
            }

            dealerId = vehicle.DealerId;
        }
        else
        {
            if (!request.DealerId.HasValue)
            {
                throw new BadRequestException(
                    "Dealer is required for general inquiry.");
            }

            var dealerExists = await _dbcontext.Dealers
                .AsNoTracking()
                .AnyAsync(o =>
                    o.Id == request.DealerId.Value);

            if (!dealerExists)
            {
                throw new NotFoundException(
                    "Dealer does not exist.");
            }

            dealerId = request.DealerId.Value;
        }

        var existingInquiry =
            await _dbcontext.Inquiries
                .AnyAsync(o =>
                    o.BuyerId == buyerId &&
                    o.DealerId == dealerId &&
                    o.VehicleId == request.VehicleId &&
                    o.Type == request.InquiryType &&
                    o.Status != InquiryStatus.Closed);

        if (existingInquiry)
        {
            throw new BadRequestException(
                "An active inquiry already exists.");
        }

        var inquiry = new Inquiry
        {
            Id = Guid.NewGuid(),
            BuyerId = buyerId,
            DealerId = dealerId,
            VehicleId = request.VehicleId,
            Message = request.Message,
            Type = request.InquiryType,
            Status = InquiryStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _dbcontext.Inquiries
            .AddAsync(inquiry);

        //var analytics = await _dbcontext.Analytics
        //    .FirstOrDefaultAsync(o => o.VehicleId == request.VehicleId);

        //if (analytics != null)
        //{
        //    analytics.InquiryCount++;
        //}

        await _dbcontext.SaveChangesAsync();

        await _cacheService.IncrementAsync($"vehicle:{request.VehicleId}:inquiries");
    }

    public async Task<PaginatedResponse<InquiryResponse>> GetDealerInquiriesAsync(Guid userId, InquirySearchRequest request)
    {
        var dealer = await _dbcontext.Dealers
            .AsNoTracking()
            .FirstOrDefaultAsync(o =>
                o.UserId == userId);

        if (dealer == null)
        {
            throw new NotFoundException("Dealer does not exist.");
        }

        request.PageSize = Math.Min(request.PageSize, 50);

        var query = _dbcontext.Inquiries
            .AsNoTracking()
            .Where(o =>
                o.DealerId == dealer.Id);

        if (request.Status.HasValue)
        {
            query = query.Where(o =>
                o.Status == request.Status);
        }

        if (request.InquiryType.HasValue)
        {
            query = query.Where(o =>
                o.Type == request.InquiryType);
        }

        var totalRecords =
            await query.CountAsync();

        var inquiries = await query
            .OrderByDescending(o =>
                o.CreatedAt)
            .Skip(
                (request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new InquiryResponse
            {
                Id = o.Id,
                VehicleId = o.VehicleId,
                VehicleTitle =
                    o.Vehicle != null
                        ? o.Vehicle.Title
                        : string.Empty,
                InquiryType = o.Type.ToString(),
                Status = o.Status.ToString(),
                Message = o.Message,
                DealerMessage = o.DealerMessage,
                CreatedAt = o.CreatedAt,
                BuyerName = o.Buyer.Name
            })
            .ToListAsync();

        return new PaginatedResponse<InquiryResponse>
        {
            Items = inquiries,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages =
                (int)Math.Ceiling(
                    totalRecords /
                    (double)request.PageSize)
        };
    }

    public async Task<InquiryResponse> GetInquiryByIdAsync(Guid inquiryId, Guid userId)
    {
        var inquiry = await _dbcontext.Inquiries
            .AsNoTracking()
            .Include(o => o.Buyer)
            .Include(o => o.Vehicle)
            .Include(o => o.Dealer)
            .FirstOrDefaultAsync(o =>
                o.Id == inquiryId);

        if (inquiry == null)
        {
            throw new NotFoundException(
                "Inquiry not found.");
        }

        var dealer = await _dbcontext.Dealers
            .AsNoTracking()
            .FirstOrDefaultAsync(o =>
                o.UserId == userId);

        var isBuyer =
            inquiry.BuyerId == userId;

        var isDealer =
            dealer != null &&
            inquiry.DealerId == dealer.Id;

        if (!isBuyer && !isDealer)
        {
            throw new ForbiddenException(
                "You do not have access to this inquiry.");
        }

        return new InquiryResponse
        {
            Id = inquiry.Id,
            VehicleId = inquiry.VehicleId,
            VehicleTitle =
                inquiry.Vehicle?.Title ??
                string.Empty,
            InquiryType = inquiry.Type.ToString(),
            Status = inquiry.Status.ToString(),
            Message = inquiry.Message,
            DealerMessage =
                inquiry.DealerMessage,
            CreatedAt = inquiry.CreatedAt,
            BuyerName =
                inquiry.Buyer.Name
        };
    }

    public async Task<PaginatedResponse<InquiryResponse>> GetMyInquiriesAsync(Guid buyerId, InquirySearchRequest request)
    {
        request.PageSize = Math.Min(request.PageSize, 50);

        var query = _dbcontext.Inquiries
            .AsNoTracking()
            .Where(o => o.BuyerId == buyerId);

        if (request.Status.HasValue)
        {
            query = query.Where(o =>
                o.Status == request.Status);
        }

        if (request.InquiryType.HasValue)
        {
            query = query.Where(o =>
                o.Type == request.InquiryType);
        }

        var totalRecords =
            await query.CountAsync();

        var inquiries = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip(
                (request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new InquiryResponse
            {
                Id = o.Id,
                VehicleId = o.VehicleId,
                VehicleTitle =
                    o.Vehicle != null
                        ? o.Vehicle.Title
                        : string.Empty,
                InquiryType = o.Type.ToString(),
                Status = o.Status.ToString(),
                Message = o.Message,
                DealerMessage = o.DealerMessage,
                CreatedAt = o.CreatedAt,
                BuyerName = o.Buyer.Name
            })
            .ToListAsync();

        return new PaginatedResponse<InquiryResponse>
        {
            Items = inquiries,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages =
                (int)Math.Ceiling(
                    totalRecords /
                    (double)request.PageSize)
        };
    }

    public async Task UpdateInquiryAsync(Guid inquiryId, UpdateInquiryRequest request, Guid userId)
    {
        var dealer = await _dbcontext.Dealers
            .AsNoTracking()
            .FirstOrDefaultAsync(o =>
                o.UserId == userId);

        if (dealer == null)
        {
            throw new NotFoundException(
                "Dealer does not exist.");
        }

        var inquiry = await _dbcontext.Inquiries
            .FirstOrDefaultAsync(o =>
                o.Id == inquiryId);

        if (inquiry == null)
        {
            throw new NotFoundException("Inquiry not found.");
        }

        if (inquiry.DealerId != dealer.Id)
        {
            throw new ForbiddenException("Inquiry does not belong to dealer.");
        }

        inquiry.Status = request.Status;

        inquiry.DealerMessage = request.DealerMessage;

        await _dbcontext.SaveChangesAsync();
    }
}
