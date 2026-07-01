using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoHub.Infrastructure.Persistence.Configurations;

public class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
{
    public void Configure(EntityTypeBuilder<Inquiry> builder)
    {
        builder.HasIndex(x => x.VehicleId);

        builder.HasIndex(x => x.BuyerId);

        builder.HasIndex(x => x.DealerId);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Type);
    }
}
