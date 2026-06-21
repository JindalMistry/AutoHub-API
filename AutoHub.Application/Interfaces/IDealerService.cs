using AutoHub.Application.DTOs.Dealers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.Interfaces;

public interface IDealerService
{
    Task<DealerResponse> CreateDealerProfileAsync(CreateDealerProfileRequest request, Guid userId);

    Task ApproveDealerAsync(Guid dealerId, Guid adminUserId);

    Task<List<DealerResponse>> GetPendingDealersAsync();

    Task RejectDealerAsync(Guid dealerId);
}
