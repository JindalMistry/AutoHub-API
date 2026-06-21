using AutoHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Application.DTOs.Dealers;

public class DealerResponse
{
    public Guid Id { get; set; }

    public string BusinessName { get; set; } = string.Empty;

    public DealerStatus Status { get; set; }
}
