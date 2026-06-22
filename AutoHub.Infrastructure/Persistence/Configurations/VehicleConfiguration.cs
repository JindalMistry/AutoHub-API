using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoHub.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasIndex(x => x.Make);

        builder.HasIndex(x => x.Model);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Price);

        builder.HasIndex(x => x.Year);

        builder.HasIndex(x => x.DealerId);

        builder.HasIndex(x => x.FuelType);

        builder.HasIndex(x => x.Transmission);

        builder.HasIndex(x => x.CreatedAt);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.RegNo)
            .IsUnique();
    }
}
