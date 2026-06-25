using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Favourite;

public class AddFavouriteRequest
{
    public Guid VehicleId { get; set; }
}