using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoHub.Infrastructure.Persistence.Configurations;

public class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {
        builder.HasKey(x => new
        {
            x.UserId,
            x.VehicleId
        });

        builder.HasIndex(o => o.UserId);

        builder.HasIndex(o => o.VehicleId);
    }
}

