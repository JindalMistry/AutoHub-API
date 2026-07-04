using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoHub.Infrastructure.Persistence.Configurations;

public class VehicleTagConfiguration : IEntityTypeConfiguration<VehicleTag>
{
    public void Configure(EntityTypeBuilder<VehicleTag> builder)
    {
        builder.HasKey(x => new
        {
            x.VehicleId,
            x.TagId
        });
    }
}