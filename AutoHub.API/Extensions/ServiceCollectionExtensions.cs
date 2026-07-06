using AutoHub.API.Configurations;
using AutoHub.Application.Configurations;
using AutoHub.Application.Interfaces;
using AutoHub.Infrastructure.Configuration;
using AutoHub.Infrastructure.Services;
using AutoHub.Infrastructure.Services.Authentication;

namespace AutoHub.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IDealerService, DealerService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IVehicleImageService, VehicleImageService>();
            services.AddScoped<IFavouriteService, FavouriteService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IInquiryService, InquiryService>();
            services.AddScoped<IBackgroundJobService, BackgroundJobService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IAdminService, AdminService>();

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            services.Configure<MinioSettings>(configuration.GetSection("Minio"));
            services.Configure<HangfireSettings>(configuration.GetSection("Hangfire"));

            return services;
        }
    }
}
