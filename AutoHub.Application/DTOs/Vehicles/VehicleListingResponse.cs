using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Vehicles;

public class VehicleListingResponse
{
    public Guid Id { get; set; }

    public Guid DealerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string Variant { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Year { get; set; }

    public int Mileage { get; set; }

    public string? ThumbnailUrl { get; set; }

    public bool IsFavourite { get; set; }
}
