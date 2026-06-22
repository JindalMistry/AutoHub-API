using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Vehicles;

public class VehicleResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public VehicleStatus Status { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Variant {  get; set; } = string.Empty;
}
