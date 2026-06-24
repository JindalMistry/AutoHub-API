using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Persistence.Configurations;

public class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
{
    public void Configure(EntityTypeBuilder<VehicleImage> builder)
    {
        builder.HasIndex(o => o.VehicleId);

        builder.HasKey(o => new
        {
            o.DisplayOrder,
            o.VehicleId
        });
    }
}
