using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Admin;

public class TopDealerResponse
{
    public Guid DealerId { get; set; }

    public string BusinessName { get; set; } = string.Empty;

    public int TotalVehicles { get; set; }

    public long TotalViews { get; set; }

    public long TotalFavourites { get; set; }

    public long TotalInquiries { get; set; }

    public long TotalReservations { get; set; }

    public decimal TotalTrendingScore { get; set; }
}
