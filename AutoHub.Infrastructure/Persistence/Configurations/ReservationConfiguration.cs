using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoHub.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasIndex(x => x.VehicleId);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.ExpiresAt);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(o => o.VehicleId)
                .IsUnique()
                .HasFilter("\"Status\" = 0");
    }
}
