using AutoHub.Domain.Enums;

namespace AutoHub.Application.DTOs.Vehicles;

public class VehicleSearchRequest
{
    public string? SearchTerm { get; set; }

    public string? Make { get; set; }

    public string? Model { get; set; }

    public string? Variant { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public int? MinYear { get; set; }

    public int? MaxYear { get; set; }

    public FuelType? FuelType { get; set; }

    public TransmissionType? Transmission { get; set; }

    public VehicleSortBy SortBy { get; set; } = VehicleSortBy.Newest;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
