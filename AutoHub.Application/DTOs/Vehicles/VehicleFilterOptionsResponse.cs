using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Vehicles;

public class VehicleFilterOptionsResponse
{
    public List<string> Makes { get; set; } = [];

    public List<string> Models { get; set; } = [];

    public List<string> Variants { get; set; } = [];

    public List<EnumOptionResponse> FuelTypes { get; set; }
        = [];

    public List<EnumOptionResponse> Transmissions { get; set; }
        = [];

    public decimal MinPrice { get; set; }

    public decimal MaxPrice { get; set; }
}
