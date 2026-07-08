using AutoHub.Application.DTOs.Dealers;
using AutoHub.Application.Interfaces;
using AutoHub.Application.Exceptions;
using AutoHub.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AutoHub.Domain.Entities;
using AutoHub.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AutoHub.Infrastructure.Services;

public class DealerService : IDealerService
{
    private readonly ApplicationDbcontext _dbContext;
    private readonly ILogger<DealerService> _logger;
    
    public DealerService (ApplicationDbcontext dbcontext, ILogger<DealerService> logger) 
    { 
        _dbContext = dbcontext;
        _logger = logger;
    }

    public async Task ApproveDealerAsync(Guid dealerId, Guid adminUserId)
    {
        var dealer = await _dbContext.Dealers
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == dealerId);

        if (dealer == null)
        {
            throw new NotFoundException("Dealer does not exist.");
        }

        if (dealer.Status != DealerStatus.Pending)
        {
            throw new BadRequestException("Dealer is not pending approval.");
        }

        dealer.Status = DealerStatus.Approved;
        dealer.ApprovedAt = DateTime.UtcNow;
        dealer.ApprovedByUserId = adminUserId;
        dealer.User.Role = UserRole.Dealer;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Admin {AdminId} approved dealer {DealerId}.",
            adminUserId,
            dealerId);
    }

    public async Task<DealerResponse> CreateDealerProfileAsync(CreateDealerProfileRequest request, Guid userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var existingDealer = await _dbContext.Dealers.FirstOrDefaultAsync(o => o.UserId == userId);

        if (existingDealer != null)
        {
            throw new BadRequestException("Dealer already exists!");
        }

        var dealer = new Dealer
        {
            Id = new Guid(),
            UserId = userId,
            BusinessName = request.BusinessName,
            Phone = request.Phone,
            Country = request.Country,
            City = request.City,
            Pincode = request.Pincode,
            Status = DealerStatus.Pending,
        };

        _dbContext.Dealers.Add(dealer);

        await _dbContext.SaveChangesAsync();

        return new DealerResponse
        {
            Id = dealer.Id,
            BusinessName = dealer.BusinessName,
            Status = dealer.Status
        };
    }

    public async Task<List<DealerResponse>> GetPendingDealersAsync()
    {
        return await _dbContext.Dealers
            .Where(o => o.Status == DealerStatus.Pending)
            .Select(o => new DealerResponse
            {
                Id = o.Id,
                BusinessName = o.BusinessName,
                Status = o.Status,
                City = o.City,
                Country = o.Country,
                Phone = o.Phone,
                Pincode = o.Pincode
            })
            .ToListAsync();
    }

    public async Task RejectDealerAsync(Guid dealerId, Guid adminId)
    {
        var dealer = await _dbContext.Dealers.FirstOrDefaultAsync(o => o.Id == dealerId);

        if (dealer == null)
        {
            throw new NotFoundException("Dealer not found");
        }

        dealer.Status = DealerStatus.Rejected;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Admin {AdminId} rejected dealer {DealerId}.",
            adminId,
            dealerId);
    }
}
