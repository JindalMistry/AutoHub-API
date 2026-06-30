using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Reservation;

public class CreateReservationRequest
{
    public Guid VehicleId { get; set; }
}
