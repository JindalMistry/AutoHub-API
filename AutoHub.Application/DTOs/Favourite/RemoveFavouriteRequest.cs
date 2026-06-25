using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Favourite;

public class RemoveFavouriteRequest
{
    public Guid VehicleId { get; set; }
}