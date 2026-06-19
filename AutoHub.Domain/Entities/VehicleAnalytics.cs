namespace AutoHub.Domain.Entities;

public class VehicleAnalytics
{
    public Guid VehicleId { get; set; }

    public int ViewCount { get; set; }

    public int FavoriteCount { get; set; }

    public int InquiryCount { get; set; }

    public int ReservationCount { get; set; }

    public decimal TrendingScore { get; set; }

    public DateTime LastCalculatedAt { get; set; }

    public Vehicle Vehicle { get; set; } = null!;
}