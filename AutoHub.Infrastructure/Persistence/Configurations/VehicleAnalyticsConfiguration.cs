using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoHub.Infrastructure.Persistence.Configurations;

public class VehicleAnalyticsConfiguration : IEntityTypeConfiguration<VehicleAnalytics>
{
    public void Configure(EntityTypeBuilder<VehicleAnalytics> builder)
    {
        builder.HasKey(x => x.VehicleId);
    }
}