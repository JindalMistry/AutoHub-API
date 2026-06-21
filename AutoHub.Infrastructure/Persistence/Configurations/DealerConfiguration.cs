using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoHub.Infrastructure.Persistence.Configurations;

public class DealerConfiguration : IEntityTypeConfiguration<Dealer>
{
    public void Configure(EntityTypeBuilder<Dealer> builder)
    {
        builder.HasIndex(o => o.Status);

        builder.HasIndex(o => o.Country);

        builder.HasIndex(o => o.City);

        builder.HasIndex(o => o.BusinessName);

        builder.HasIndex(o => o.UserId)
            .IsUnique();
    }
}