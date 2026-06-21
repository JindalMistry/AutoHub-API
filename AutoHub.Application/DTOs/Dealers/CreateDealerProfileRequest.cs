using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Dealers;

public class CreateDealerProfileRequest
{
    public string BusinessName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Pincode { get; set; } = string.Empty;
}
