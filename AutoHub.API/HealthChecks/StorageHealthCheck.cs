using AutoHub.Application.Interfaces;
using AutoHub.Infrastructure.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace AutoHub.API.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IStorageService _storageService;
        private readonly StorageSettings storageSettings;

        public StorageHealthCheck(
            IStorageService storageService, 
            IOptions<StorageSettings> storageSettings)
        {
            _storageService = storageService;
            this.storageSettings = storageSettings.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            var bucketName = storageSettings.BucketName;
            var provider = storageSettings.Provider;

            try
            {
                var exists = await _storageService.IsHealthyAsync(bucketName, cancellationToken);

                return exists
                    ? HealthCheckResult.Healthy($"{provider} is healthy.")
                    : HealthCheckResult.Unhealthy("Bucket does not exist.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    $"Unable to connect to {provider}.",
                    ex);
            }
        }
    }
}
