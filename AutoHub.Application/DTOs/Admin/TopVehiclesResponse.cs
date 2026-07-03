using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Admin;

public class TopVehiclesResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public long Views { get; set; } = 0;
    public long Favourites { get; set; } = 0;
    public long Inquiries { get; set; } = 0;
    public long Reservations { get; set; } = 0;
    public long TrendingScore { get; set; } = 0;
}
