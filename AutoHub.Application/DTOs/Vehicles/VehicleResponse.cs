using AutoHub.Application.DTOs.Dealers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Vehicles;

public class VehicleResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Variant {  get; set; } = string.Empty;
    public int Mileage { get; set; }
    public int Year { get; set; }
    public string Transmission { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public DealerResponse? Dealer { get; set; }
    public bool IsFavourite { get; set; }
}
