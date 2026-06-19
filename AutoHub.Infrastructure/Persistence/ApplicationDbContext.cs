using AutoHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoHub.Infrastructure.Persistance;

public class ApplicationDbcontext : DbContext
{
    public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Dealer> Dealers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Favourite> Favourites { get; set; }
    public DbSet<Inquiry> Inquiries { get; set; }
    public DbSet<VehicleImage> VehicleImages { get; set; }
    public DbSet<VehicleAnalytics> Analytics { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<VehicleTag> VehicleTags { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbcontext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}